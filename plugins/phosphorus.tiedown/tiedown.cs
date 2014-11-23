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
_nodes9:node:""""
_nodes4:node:
_nodes8  :  node  :     @            
_nodes7:node:@
_nodes6:node:tjalla
_nodes5:node:@""tmp:5
  str:@""""howdy """"""""tjobing
world""""""
_integer   :   int   :   5
_integer:int16:5656
_single:single:999.437
_double:double:999.437
_decimal:decimal:100.567
_bool:bool:true
_byte:byte:115
_bool:char:%
_date:date:""2014-11-25T23:59:57.876""
_date:date:""2014-11-25T23:59:57""
_date:date:  2014-12-24    
_path:  path :0
_nodes:node:    @""tmp:5
  jo:dude!!""          
_nodes2:node:""@""
_nodes3:node:@
_data:liv
  flercellede:    dyr    
    reptiler:slanger, osv
    pattedyr:hunder, mennesker, osv
      kattedyr:    ""pusekatter, tigre, løver og sånt no :)""            
        tigre            :                                          bengalsk
        løver:afrika
      primater:apekatter, osv
        mennesker:per, ole og jens
          johan:kult!!
      hundedyr:coyote, ulv, hund
        ulv:grå
        coyote:ørken
_x
  for-each:@/../_data/**/%2/(/>/&/</)/?node
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
  and:@/./+/+/+/?name
    =:mumbo
    and:@/././+/+/+/?value
      =:jumbo
    or:@/././+/+/+/?value
      =:jumbo5
      or:@/./././+/+/+/?value
        =:jumbo2
      and:@/../**/_xxx/?value
        =:thomas
      and:@/../**/_xxx/?value
        =:thomas
      or:@/../**/_xxx/?value
        =:thomas2
  lambda
    set:@/././+/+/+/?value
      :conditional branching was true
else-if:@/+/+/?name
  =:mumbo
  and:@/./+/+/?value
    =:jumbo2
  lambda
    set:@/././+/+/?value
      :else-if branching was true
else
  lambda
    set:@/././+/?value
      :else was struck
mumbo:jumbo7
_xxx:thomas3
_zzz
  one:1
  two:2
  three:3
while:@/-/*/?node
  lambda
    add:@/../**/_ccc/?node
      :@/../**/_zzz/0/?node
    set:@/../**/_zzz/0/?node
_ccc
pf.file.save";
            // //////////////////////////////////////////////////////////////////////////////////////////////////////////
            // / operators; "=", "!=", ">", "<", ">=", "<="
            // / "or" && "and"
            // if (name='lambda' || {name='pf.lambda' && 
            //        name2='mumbo' && (value2='jumbo' || value2='jumbo5' || ({value2='jumbo2' && value3='thomas'} || value3='thomas2'))})
            // //////////////////////////////////////////////////////////////////////////////////////////////////////////
            Node node = new Node ("root", code);
            context.Raise ("pf.code-2-nodes", node);
            node.Value = null;
            context.Raise ("lambda", node);
            context.Raise ("pf.nodes-2-code", node);
        }
    }
}

