/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.lambda
{
    /// <summary>
    /// class responsible for creating, deleting, overriding and removing overrides of Active Events
    /// </summary>
    public static class events
    {
        // contains our list of dynamically created Active Events
        private static Dictionary<string, List<Event>> _events = new Dictionary<string, List<Event>> ();

        // contains our list of overrides
        private static List<Override> _overrides = new List<Override> ();

        /// <summary>
        /// creates a new Active Event
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "event")]
        private static void lambda_event (ApplicationContext context, ActiveEventArgs e)
        {
            // verifying syntax
            if (e.Args.Count > 2 || e.Args.Count == 0)
                throw new ArgumentException ("syntax error in [event], only two arguments are legal; [overrides] and [code], and [code] is mandatory");
            else if (e.Args.Count == 2 && (e.Args [0].Name != "overrides" || e.Args [1].Name != "code"))
                throw new ArgumentException ("syntax error in [event], only [overrides] and [code] are legal arguments, and [overrides] must come before [code]");
            else if (e.Args.Count == 1 && e.Args [0].Name != "code")
                throw new ArgumentException ("syntax error in [event], [code] is mandatory argument");

            // retrieving [name]
            string name = e.Args.Get<string> ();
            if (string.IsNullOrEmpty (name))
                throw new ArgumentException ("no event name given to [event]");

            // retrieving [overrides] and [code]
            string overrides = null;
            Node lambda = null;
            if (e.Args.Count == 2) {
                overrides = e.Args [0].Get<string> ();
                lambda = e.Args [1].Clone ();
            } else {
                lambda = e.Args [0].Clone ();
            }

            // creating event
            CreateEvent (context, name, lambda);

            // creating override, if any
            if (!string.IsNullOrEmpty (overrides))
                CreateOverride (context, name, overrides);
        }

        /// <summary>
        /// deletes all existing events with the given name
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "delete-event")]
        private static void lambda_delete_event (ApplicationContext context, ActiveEventArgs e)
        {
            // retrieving [name]
            string name = e.Args.Get<string> ();
            if (string.IsNullOrEmpty (name))
                throw new ArgumentException ("no event name given to [delete-event]");

            // verifying syntax
            if (e.Args.Count > 0)
                throw new ArgumentException ("[delete-event] does not take any arguments");

            DeleteEvent (context, name);
        }

        /// <summary>
        /// responsible for "re-mapping" all dynamically created Active Events and overrides
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.core.initialize-application-context")]
        private static void pf_core_initialize_application_context (ApplicationContext context, ActiveEventArgs e)
        {
            // initializing all Active Event lambda objects
            foreach (var idxKey in _events.Keys) {
                foreach (var evt in _events [idxKey]) {
                    InitializeEvent (context, evt);
                }
            }

            // initializing all overrides
            foreach (var idxOverride in _overrides) {
                InitializeOverride (context, idxOverride);
            }
        }
        
        /*
         * responsible for executing all dynamically created Active Events or lambda objects
         */
        [ActiveEvent (Name = "_pf.core.execute-dynamic-lambda")]
        private static void _pf_core_execute_dynamic_lambda (ApplicationContext context, ActiveEventArgs e)
        {
            string actualEvtName = e.Base.Name;
            if (_events.ContainsKey (actualEvtName)) {
                foreach (var evt in _events [actualEvtName]) {
                    Node exe = evt.Lambda.Clone ();
                    foreach (Node idxArg in e.Args.Children) {
                        exe.Add (idxArg.Clone ());
                    }
                    context.Raise ("lambda", exe);
                }
            }
        }

        /*
         * creates new Active Event sink
         */
        private static void CreateEvent (ApplicationContext context, string name, Node lambda)
        {
            Event evt = new Event (name, lambda);
            if (!_events.ContainsKey (name))
                _events [name] = new List<Event> ();
            _events [name].Add (evt);
            InitializeEvent (context, evt);
        }

        /*
         * initializes Active Event by creating the mapping necessary to execute lambda object when Active Event is raised
         */
        private static void InitializeEvent (ApplicationContext context, Event evt)
        {
            if (!context.HasOverride (evt.Name, "_pf.core.execute-dynamic-lambda"))
                context.Override (evt.Name, "_pf.core.execute-dynamic-lambda");
        }

        /*
         * deletes the given event
         */
        private static void DeleteEvent (ApplicationContext context, string evt)
        {
            _events.Remove (evt);
            context.RemoveOverride (evt, "_pf.core.execute-dynamic-lambda");
        }

        /*
         * creates an override from one Active Event to another
         */
        private static void CreateOverride (ApplicationContext context, string overrideEvent, string overriddenEvent)
        {
            Override over = new Override (overrideEvent, overriddenEvent);
            _overrides.Add (over);
            InitializeOverride (context, over);
        }

        /*
         * initializes override by creating the necessary mapping on the application context to make sure override is 
         * invoked correctly
         */
        private static void InitializeOverride (ApplicationContext context, Override over)
        {
            context.RemoveOverride (over.OverriddenEvent, "_pf.core.execute-dynamic-lambda");
            context.Override (over.OverriddenEvent, over.OverrideEvent);
            context.Override (over.OverrideEvent, "_pf.core.execute-dynamic-lambda");
        }
    }
}

