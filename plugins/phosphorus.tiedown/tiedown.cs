/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using phosphorus.core;

namespace phosphorus.tiedown
{
    /// <summary>
    /// class to help tie together application-start Active Event and page-load Active Event.  loads and executes the startup hyperlisp file
    /// and the page-load hyperlisp file
    /// </summary>
    public static class tiedown
    {
        /// <summary>
        /// executes the startup hyperlisp files
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.application-start")]
        private static void pf_application_start (ApplicationContext context, ActiveEventArgs e)
        {
            // execute startup hyperlisp file
        }

        /// <summary>
        /// executes the page-load hyperlisp file
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.page-load")]
        private static void pf_page_load (ApplicationContext context, ActiveEventArgs e)
        {
            // execute load hyperlisp file
            string code = @"
_data:liv
  flercellede:dyr
    reptiler:slanger, osv
    pattedyr:hunder, mennesker, osv
      kattedyr:pusekatter, tigre, løver og sånt no :)
        tigre:bengalsk
        løver:afrika
      primater:apekatter, osv
        mennesker:per, ole og jens
          johan:kult!!
      hundedyr:coyote, ulv, hund
        ulv:grå
        coyote:ørken
_x
  for-each:@/../0/**/%2/(/>/&/</)/?node
    set:@/+/+/0/?value
      :@/""..for-each""/__dp/#/?value
    set:@/""..for-each""/__dp/#/?value
      :@/"".._x""/_name/#/?value
    add:@/././_name/#/?node
      node
lambda:@/-/?node
  _name:thomas hansen ER KUUUUUUUUUUUUUUUUUUUL!!!!!!!
_z:fff
set:@/-/./(/_z/&/=fff/&/=""/f?f/""/&/""/_?z/""/)/?value
  :FOUND MATCH!!
_z_code:@""set:@/../_return/#/?value
  :howdy world""
pf.lambda:@/-/?value
  _return:before
if:@/-/?name
  =:lambda
  or:@/./-/?name
    =:pf.lambda
  and:@/./+/?name
    =:mumbo
    and:@/././+/?value
      =:jumbo
    or:@/././+/?value
      =:jumbo5
      or:@/./././+/?value
        =:jumbo2
      and:@/../**/_xxx/?value
        =:thomas
      or:@/../**/_xxx/?value
        =:thomas2
  lambda
    set:@/././+/?value
      :conditional branching was true
mumbo:jumbo2
_xxx:thomas2
pf.file.save
";
            // //////////////////////////////////////////////////////////////////////////////////////////////////////////
            // / operators; "=", "!=", ">", "<", ">=", "<="
            // / "or" && "and"
            // //////////////////////////////////////////////////////////////////////////////////////////////////////////
            Node node = new Node ("root", code);
            context.Raise ("pf.code-2-nodes", node);
            node.Value = null;
            context.Raise ("lambda", node);
            context.Raise ("pf.nodes-2-code", node);
        }
    }
}

