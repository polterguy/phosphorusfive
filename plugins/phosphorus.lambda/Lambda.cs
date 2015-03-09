/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System.Collections.Generic;
using System.Linq;
using phosphorus.core;
using phosphorus.expressions;
using phosphorus.expressions.exceptions;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace phosphorus.lambda
{
    /// <summary>
    ///     Class wrapping all [lambda.xxx] keywords in pf.lambda.
    /// 
    ///     The [lambda.xxx] keywords allows you to execute some specific piece of pf.lambda code.
    /// </summary>
    public static class Lambda
    {
        /// <summary>
        ///     Executes a specified piece of pf.lambda block.
        /// 
        ///     The [lambda] keyword can either take a list of children nodes, or an 
        ///     <see cref="phosphorus.expressions.Expression">Expression</see> as its source, or both. If you have no value in your
        ///     [lambda] node, then the children nodes of your [lambda] node will be executed as pf.lambda nodes. For instance;
        /// 
        ///     <pre>lambda
        ///   set:@/.?value
        ///     source:success</pre>
        /// 
        ///     You can alternatively invoke [lambda] by giving it an expression, instead of a list of children nodes. If you do, then
        ///     the result of that expression will be executed instead of its children nodes, passing in any children as <em>"arguments"</em>
        ///     to your pf.lambda execution. For instance;
        /// 
        ///     <pre>_exe
        ///   set:@/.?value
        ///     source:Hello {0}
        ///       :@/./././"*"/_input?value
        /// lambda:@/-?node
        ///   _input:Thomas</pre>
        /// 
        ///     Please notice in the above code, that you do not supply an expression leading directly to the nodes that you wish to execute, 
        ///     but instead the expression leads to the node that <em>"wraps"</em> your pf.lambda nodes to be executed. Meaning, [_exe] for the
        ///     above example. You can also of course supply an expression yielding multiple results, for instance;
        /// 
        ///     <pre>_exe1
        ///   set:@/.?value
        ///     source:Hello {0}
        ///       :@/./././"*"/_input?value
        /// _exe2
        ///   set:@/.?value
        ///     source:Howdy {0}
        ///       :@/./././"*"/_input?value
        /// lambda:@/-|/-2?node
        ///   _input:Thomas</pre>
        /// 
        ///     Anything you submit to [lambda] that is not a node-set, will be converted into a node-set, before executed. Consider the 
        ///     following code;
        /// 
        ///     <pre>lambda:\@"set:@/./""*""/_result/#?name
        ///   source:Hello there {0}
        ///     :@/./././""*""/_input?value"
        ///   _result:node:
        ///   _input:Thomas Hansen</pre>
        /// 
        ///     The above code might be difficult to understand when you start out with pf.lambda. However, to explain it, it basically passes in
        ///     the [_result] node by reference to the pf.lambda block executed by the [lambda] statement. The [lambda] block itself, will be 
        ///     created from the string, which is the value of the above [lambda] node, converted into a pf.lambda node-set. In addition it 
        ///     passes in the [_input] parameter. Since we are executing our pf.lambda block, which is converted from text above, the only way 
        ///     to return values from that [lambda] block, is to pass in nodes by reference, for then to have the [lambda] block modify the 
        ///     contents of that node, and/or node hierarchy.
        /// 
        ///     A highly useful example of why you'd like to use constructs such as the above, is to realize you can use the above logic to
        ///     load pf.lambda files, for then to pass in arguments to the execution of those pf.lambda files. Consider the following code;
        /// 
        ///     <pre>pf.file.load:some-hyperlisp-file.hl
        /// lambda:@/./0?value
        ///   some-input-parameter:foo
        ///   some-return-value:node:</pre>
        /// 
        ///     The above construct allows you to perceive files as <em>"functions"</em>, and pass in and return arguments to and from these files.
        ///     This is a pattern often used when you use pf.lambda and Phosphorus.Five. In fact, that's where the name pf.lambda comes from, since
        ///     in pf.lambda, everything is a potential executable piece of <em>"code"</em>.
        /// 
        ///     Another highly useful trait of constructs such as the above, is that you can pass code across HTTP requests, and boundaries where
        ///     execution-trees normally don't travel easily, which facilitates for passing pieces of code from one server on your intranet, to
        ///     another server, which creates better scaling-out capabilities for your end solutions.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "lambda")]
        private static void lambda_lambda (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Name == "lambda" && e.Args.Value != null) {
                // executing a value object, converting to node, before we pass into execution method,
                // making sure we pass in children of [lambda] as "arguments" or "parameters" to [lambda] statement
                foreach (var idxSource in XUtil.Iterate<Node> (e.Args, context)) {
                    ExecuteBlockNormal (context, idxSource, e.Args.Children);
                }
            } else {
                // executing current scope
                ExecuteBlockNormal (context, e.Args, new Node[] {});
            }
        }

        /// <summary>
        ///     Executes a specified piece of pf.lambda block.
        /// 
        ///     The [lambda.invoke] keyword, is similar to the <see cref="phosphorus.lambda.Lambda.lambda_lambda">[lambda]</see> keyword, except
        ///     that it does not consider inheritence chains for first-level children Active Events. This means that you can use [lambda.invoke]
        ///     to call <em>"base Active Events"</em>, where you have Active Events that are overridden by other Active Events. Consider the
        ///     following code;
        /// 
        ///     <pre>remove-event:foo
        /// remove-event:bar
        /// remove-override:foo
        ///   super:bar
        /// event:foo
        ///   lambda
        ///     set:@/./.?value
        ///       source:success
        /// event:bar
        ///   lambda
        ///     set:@/./.?value
        ///       source:error
        /// override:foo
        ///   super:bar
        /// lambda.invoke
        ///   foo</pre>
        /// 
        ///     If you change the above [lambda.invoke] to become [lambda], then the [foo] node at the end, will end up having the value of 
        ///     <em>"error"</em> instead of <em>"success"</em>. This is because the overridden [bar] Active Event will be invoked instead
        ///     of the base Active Event of [foo], since [bar] overrides [foo]. By using [lambda.invoke], you can create blocks of pf.lambda
        ///     code, that will not consider the inheritence chains of any first-level Active Event invocations beneath themselves, but instead
        ///     call the Active Events directly. This allows you to create <em>"call base functionality"</em> in your pf.lambda code.
        /// 
        ///     Besides from that, the [lambda.invoke] keyword works identically to [lambda]. Check out the documentation for 
        ///     <see cref="phosphorus.lambda.Lambda.lambda_lambda">[lambda]</see> to see more extensive documentation for what you can do with 
        ///     [lambda.invoke].
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "lambda.invoke")]
        private static void lambda_invoke (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Name == "lambda.invoke" && e.Args.Value != null) {
                // executing a value object, converting to node, before we pass into execution method,
                // making sure we pass in children of [lambda] as "arguments" or "parameters" to [lambda] statement
                foreach (var idxSource in XUtil.Iterate<Node> (e.Args, context)) {
                    ExecuteBlockNormal (context, idxSource, e.Args.Children, true);
                }
            } else {
                // executing current scope
                ExecuteBlockNormal (context, e.Args, new Node[] {}, true);
            }
        }

        /// <summary>
        ///     Executes a specified piece of pf.lambda block.
        /// 
        ///     The [lambda.immutable] keyword, works similar to the [lambda] keyword, except that whatever code you start out with, will also
        ///     be the code you end up with, since [lambda.immutable] will set the code block it executes back, to whatever it contained before
        ///     execution. This allows you to create code-blocks that after execution will be apparently unchanged. Consider the following code;
        /// 
        ///     <pre>
        /// lambda.immutable
        ///   _foo
        ///   set:@/./0?value
        ///     src:Howdy World</pre>
        /// 
        ///     If you change the above [lambda.immutable] statement, to become a [lambda] statement, then the value of [_foo] will become 
        ///     <em>"Hello World"</em> after execution. But since we're using [lambda.immutable], then the code block executed, will be set back
        ///     to its original state, after execution.
        /// 
        ///     Besides from that, [lambda.immutable] is identical to [lambda]. See the <see cref="phosphorus.lambda.Lambda.lambda_lambda">[lambda]</see>
        ///     keyword's documentation, to understand what more features you can use with [lambda.immutable].
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "lambda.immutable")]
        private static void lambda_immutable (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Name == "lambda.immutable" && e.Args.Value != null) {
                // executing a value object, converting to node, before we pass into execution method,
                // making sure we pass in children of [lambda] as "arguments" or "parameters" to [lambda] statement
                foreach (var idxSource in XUtil.Iterate<Node> (e.Args, context)) {
                    ExecuteBlockImmutable (context, idxSource, e.Args.Children);
                }
            } else {
                // executing current scope
                ExecuteBlockImmutable (context, e.Args, new Node[] {});
            }
        }

        /// <summary>
        ///     Executes a specified piece of pf.lambda block.
        /// 
        ///     The [lambda.copy] keyword, works similar to the [lambda] keyword, except that [lambda.copy] will create a copy of whatever pf.lambda
        ///     execution blocks you executes, with the execution blocks becoming the root nodes of you [lambda.copy] statement. Consider the 
        ///     following code;
        /// 
        ///     <pre>_out:error
        /// _exe
        ///   _out:success
        ///   set:@/./"*"/_input/#?name
        ///     source:@/../"*"/_out?value
        /// lambda.copy:@/-?node
        ///   _input:node:</pre>
        /// 
        ///     If you change the above [lambda.copy] statement, to become a [lambda] statement, then the name of the [_input] reference node 
        ///     will become <em>"error"</em> after execution. But since we're using [lambda.copy], then the root node iterator, in the [source]
        ///     parameter above, will actually access the [_exe] node, and not the root node for the entire execution tree, and hence the [_input]'s
        ///     reference node's name will be set to <em>"success"</em> and not <em>"error"</em>.
        /// 
        ///     Another thing you'll notice, is that since we're executing the [lambda.copy] on a copy of the executedd nodes, then the execution
        ///     tree will not be modified, the same way it is not modified when using [lambda.immutable], though for different reasons obviously.
        /// 
        ///     Besides from that, [lambda.copy] is identical to [lambda]. See the <see cref="phosphorus.lambda.Lambda.lambda_lambda">[lambda]</see>
        ///     keyword's documentation, to understand what more features you can use with [lambda.immutable].
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "lambda.copy")]
        private static void lambda_copy (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Name == "lambda.copy" && e.Args.Value != null) {
                // executing a value object, converting to node, before we pass into execution method,
                // making sure we pass in children of [lambda] as "arguments" or "parameters" to [lambda] statement
                foreach (var idxSource in XUtil.Iterate<Node> (e.Args, context)) {
                    ExecuteBlockCopy (context, idxSource, e.Args.Children);
                }
            } else {
                // executing current scope
                ExecuteBlockCopy (context, e.Args, new Node[] {});
            }
        }

        /// <summary>
        ///     Executes a specified piece of pf.lambda statement.
        /// 
        ///     The [lambda.single] keyword, works similar to the [lambda] keyword, except that [lambda.single] will execute one single statement,
        ///     and not a block of code. Consider the following code, and notice how we're not executing the block, but the actual [set] node as 
        ///     a <em>"single"</em> statement;
        /// 
        ///     <pre>_exe
        ///   set:\@/.?value
        ///     source:Hello World
        /// lambda.single:\@/-/0?node</pre>
        /// 
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "lambda.single")]
        private static void lambda_single (ApplicationContext context, ActiveEventArgs e)
        {
            if (string.IsNullOrEmpty (e.Args.Get<string> (context)))
                throw new LambdaException ("nothing was given to [lambda.single] for execution", e.Args, context);

            // executing a value object, converting to node, before we pass into execution method,
            // making sure we pass in children of [lambda] as "arguments" or "parameters" to [lambda] statement
            foreach (var idxSource in XUtil.Iterate<Node> (e.Args, context)) {
                ExecuteStatement (idxSource, context, true);
            }
        }

        /*
         * executes a block of nodes, this is where the actual execution happens
         * this is the "heart beat" method of the "pf.lambda" execution engine
         */
        private static void ExecuteBlockNormal (ApplicationContext context, Node exe, IEnumerable<Node> args, bool directly = false)
        {
            // passing in arguments, if there are any
            foreach (var idx in args) {
                exe.Add (idx.Clone ());
            }

            // iterating through all nodes in execution scope, and raising these as Active Events
            var idxExe = exe.FirstChild;
            while (idxExe != null) {
                // executing current statement and retrieving next execution statement
                idxExe = ExecuteStatement (idxExe, context, false, directly);
            }
        }

        /*
         * executes a block of nodes, this is where the actual execution happens
         * this is the "heart beat" method of the "pf.lambda" execution engine
         */
        private static void ExecuteBlockImmutable (ApplicationContext context, Node exe, IEnumerable<Node> args)
        {
            // storing original execution nodes, such that we can set back execution
            // block to what it originally was
            var oldNodes = exe.Children.Select (idx => idx.Clone ()).ToList ();

            // passing in arguments, if there are any
            foreach (var idx in args) {
                exe.Add (idx.Clone ());
            }

            // iterating through all nodes in execution scope, and raising these as Active Events
            var idxExe = exe.FirstChild;
            while (idxExe != null) {
                // executing current statement and retrieving next execution statement
                idxExe = ExecuteStatement (idxExe, context);
            }

            // making sure we set back execution block to original nodes
            exe.Clear ();
            exe.AddRange (oldNodes);
        }

        /*
         * executes a block of nodes, this is where the actual execution happens
         * this is the "heart beat" method of the "pf.lambda" execution engine
         */
        private static void ExecuteBlockCopy (ApplicationContext context, Node exe, IEnumerable<Node> args)
        {
            // making sure lambda is executed on copy of execution nodes, if we should,
            // without access to nodes outside of its own scope
            exe = exe.Clone ();

            // passing in arguments, if there are any
            foreach (var idx in args) {
                exe.Add (idx.Clone ());
            }

            // iterating through all nodes in execution scope, and raising these as Active Events
            var idxExe = exe.FirstChild;
            while (idxExe != null) {
                // executing current statement and retrieving next execution statement
                idxExe = ExecuteStatement (idxExe, context);
            }
        }

        /*
         * executes one execution statement
         */
        private static Node ExecuteStatement (Node exe, ApplicationContext context, bool force = false, bool directly = false)
        {
            // storing "next execution node" as fallback, to support "delete this node" pattern
            var nextFallback = exe.NextSibling;

            // we don't execute nodes that start with an underscore "_" since these are considered "data segments"
            // also we don't execute nodes with no name, since these interfers with "null Active Event handlers"
            if (force || (!exe.Name.StartsWith ("_") && exe.Name != string.Empty)) {
                if (directly) {
                    // raising the given Active Event directly, not considering inheritance chain
                    context.RaiseDirectly (exe.Name, exe);
                } else {
                    // raising the given Active Event normally, taking inheritance chain into account
                    context.Raise (exe.Name, exe);
                }
            }

            // prioritizing "NextSibling", in case this node created new nodes, while having
            // nextFallback as "fallback node", in case current execution node removed current execution node,
            // but if "current execution node" untied nextFallback, in addition to "NextSibling",
            // we return null back to caller
            return exe.NextSibling ?? (nextFallback != null && nextFallback.Parent != null &&
                                       nextFallback.Parent == exe.Parent ? nextFallback : null);
        }
    }
}
