/*
 * Phosphorus Five, copyright 2014 - 2017, Thomas Hansen, thomas@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License version 3, as published by
 * the Free Software Foundation.
 *
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Phosphorus Five.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using p5.core;
using p5.exp.exceptions;
using MySql.Data.MySqlClient;

namespace p5.mysql
{
    /// <summary>
    ///     Class wrapping [p5.mysql.transaction.begin/commit/rollback].
    /// </summary>
    public class Transaction
    {
        MySqlConnection _connection;
        MySqlTransaction _transaction;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:p5.mysql.Transaction"/> class.
        /// </summary>
        public Transaction (ApplicationContext context, Node args) {
            _connection = Connection.Active (context, args);
            _transaction = _connection.BeginTransaction ();
        }

        /// <summary>
        ///     Starts a new database transaction for the currently active (top most) MySQL connection.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.mysql.transaction.begin")]
        public static void p5_mysql_transaction_begin (ApplicationContext context, ActiveEventArgs e) {
            // Creating a new transaction instance listener.
            var transaction = new Transaction (context, e.Args);
            context.RegisterListeningInstance (transaction);

            // Evaluating lambda for current transaction, making sure it's rolled back by default, unless an explicit [p5.mysql.transaction.commit] is invoked.
            try {

                // Evaluating lambda for [p5.mysql.connect].
                context.RaiseEvent ("eval-mutable", e.Args);

            } finally {

                // Cleaning up.
                context.UnregisterListeningInstance (transaction);

                // Making sure we by default roll back transaction, unless it's already been committed.
                transaction.Finished ();
            }
        }

        /*
         * The next two Active Events are there mostly to make sure we get intellisense (registered Active Events), 
         * in addition to some basic sanity check.
         * The actual implementation of rollback and commit, occurs in private instance handlers.
         */

        /// <summary>
        ///     Rolls back a transaction.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.mysql.transaction.rollback")]
        public static void p5_mysql_transaction_rollback (ApplicationContext context, ActiveEventArgs e) {
            // Sanity check.
            if (!context.HasActiveEvent (".p5.mysql.transaction.rollback"))
                throw new LambdaException ("No active MySQL database transactions for [p5.mysql.transaction.rollback]", e.Args, context);

            // Forwarding invocation to private instance handler.
            context.RaiseEvent (".p5.mysql.transaction.rollback", e.Args);
        }

        /// <summary>
        ///     Commits a transaction.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.mysql.transaction.commit")]
        public static void p5_mysql_transaction_commit (ApplicationContext context, ActiveEventArgs e) {
            // Sanity check.
            if (!context.HasActiveEvent (".p5.mysql.transaction.commit"))
                throw new LambdaException ("No active MySQL database transactions for [p5.mysql.transaction.commit]", e.Args, context);

            // Forwarding invocation to private instance handler.
            context.RaiseEvent (".p5.mysql.transaction.commit", e.Args);
        }

        /// <summary>
        ///     Hidden implementation of database transaction rollback.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = ".p5.mysql.transaction.rollback")]
        private void _p5_mysql_transaction_rollback (ApplicationContext context, ActiveEventArgs e) {
            // Since we might have multiple instance listeners when transactions are being nested, we need to determine if the
            // current connection is the one this instance listener was registered with.
            // Otherwise we cannot intelligently nest multiple transaction objects, for multiple database connections, without
            // risking rolling back all of them, when we only want to roll back one of them.
            if (!IsForCurrent (context, e.Args))
                return;

            // Making sure transaction is not previously committed or rolled back.
            if (_transaction == null)
                throw new LambdaException ("There are no open transactions for current connection", e.Args, context);

            // Doing this such that we for sure are able to nullify private variable wrapping transaction object.
            // This is done, such that we don't risk rolling back later, if an exception occurs during Rollback.
            var trs = _transaction;
            _transaction = null;
            trs.Rollback ();
        }

        /// <summary>
        ///     Hidden implementation of database transaction commit.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = ".p5.mysql.transaction.commit")]
        private void _p5_mysql_transaction_commit (ApplicationContext context, ActiveEventArgs e) {
            // Since we might have multiple instance listeners when transactions are being nested, we need to determine if the
            // current connection is the one this instance listener was registered with.
            // Otherwise we cannot intelligently nest multiple transaction objects, for multiple database connections, without
            // risking committing all of them, when we only want to commit one of them.
            if (!IsForCurrent (context, e.Args))
                return;

            // Making sure transaction is not previously committed or rolled back.
            if (_transaction == null)
                throw new LambdaException ("There are no open transactions for current connection", e.Args, context);

            // Doing this such that we for sure are able to nullify private variable wrapping transaction object.
            // This is done, such that we don't risk rolling back later, if an exception occurs during Commit.
            var trs = _transaction;
            _transaction = null;
            trs.Commit ();
        }

        /*
         * Helper for above, to determine if current invocation is for current instance.
         */
        private bool IsForCurrent (ApplicationContext context, Node args) {
            // Making sure we perform an object reference comparison, to determine if current invocation is for current instance.
            return (object)Connection.Active (context, args) == (object)_connection;
        }

        /*
         * Executed when transaction is finished, meaning rolling back, unless _transaction is set to null.
         */
        private void Finished () {
            _transaction?.Rollback ();
        }
    }
}
