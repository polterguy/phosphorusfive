
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
        /// creates a new Active Event. pass in [lambda] and [overrides]. [overrides] can either be a 
        /// string through value, or a list of values as children
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "event")]
        private static void lambda_event (ApplicationContext context, ActiveEventArgs e)
        {
            // verifying syntax
            if (e.Args.Count > 2 || e.Args.Count == 0)
                throw new ArgumentException ("syntax error in [event], only two arguments are legal; [overrides] and [lambda], and [lambda] is mandatory");
            else if (e.Args.Count == 2 && (e.Args [0].Name != "overrides" || e.Args [1].Name != "lambda"))
                throw new ArgumentException ("syntax error in [event], only [overrides] and [lambda] are legal arguments, and [overrides] must come before [lambda]");
            else if (e.Args.Count == 1 && e.Args [0].Name != "lambda")
                throw new ArgumentException ("syntax error in [event], [lambda] is mandatory argument");

            // retrieving [name]
            string name = e.Args.Get<string> ();
            if (string.IsNullOrEmpty (name))
                throw new ArgumentException ("no event name given to [event]");

            // retrieving [overrides] and [code]
            Node lambda = null;
            if (e.Args.Count == 2) {
                CreateOverride (context, name, GetOverrides (e.Args [0]));
                lambda = e.Args [1].Clone ();
            } else {
                lambda = e.Args [0].Clone ();
            }

            // creating event
            CreateEvent (context, name, lambda);
        }

        /// <summary>
        /// deletes all existing events with the given name given through the value of the [delete-event] node
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "delete-event")]
        private static void lambda_delete_event (ApplicationContext context, ActiveEventArgs e)
        {
            // retrieving [name] of event to delete
            string name = e.Args.Get<string> ();
            if (string.IsNullOrEmpty (name))
                throw new ArgumentException ("no event name given to [delete-event]");

            // verifying syntax
            if (e.Args.Count > 0)
                throw new ArgumentException ("[delete-event] does not take any arguments");

            // actually deleting event
            DeleteEvent (context, name);
        }

        /// <summary>
        /// [call-base] calls the base Active Event, if any, for the given invocation of the currently
        /// executing Active Event. pass in arguments to base as children, otherwise base event won't
        /// have any arguments during execution
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "call-base")]
        private static void lambda_call_base (ApplicationContext context, ActiveEventArgs e)
        {
            // checking to see if current Active Event actually has a base, and if we're even inside an Active Event
            if (e.Args.Root [0].Name == "__base" && e.Args.Root [0].Count > 0) {

                // retrieving base Active Event name, making sure "current base" ise removed, but all other
                // base event data is passed onwards into the hierarchy
                string activeEventName = e.Args.Root [0] [0].Get<string> ();

                // appending base information to current invocation, and removing "this" event from base list
                // of args passed into "this invocation", but only if base has additional base events
                if (e.Args.Root [0].Count > 1) {
                    e.Args.Insert (0, e.Args.Root [0].Clone ());
                    e.Args [0] [0].Untie ();
                }

                // invoking base event
                InvokeEvent (context, activeEventName, e.Args);

                // cleaning up "base list" after execution of base, but only if there is any "base events" for current base
                if (e.Args.Count > 0 && e.Args [0].Name == "__base")
                    e.Args [0].Untie ();
            }
        }

        /// <summary>
        /// dynamically overrides one method with another. pass in the method to override as value, and
        /// the method you wish to override it with as [with]
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "override")]
        private static void lambda_override (ApplicationContext context, ActiveEventArgs e)
        {
            // verifying syntax
            if (e.Args.Count != 1 || e.Args [0].Name != "with")
                throw new ArgumentException ("you must pass in [with] to [override] as event to override with");
            if (e.Args [0].Value == null && e.Args [0].Count == 0)
                throw new ArgumentException ("you must pass in either a value or children to [override] as event(s) you wish to override");
            if (e.Args [0].Value != null && e.Args [0].Count > 0)
                throw new ArgumentException ("you must pass in either a value or children to [override] as event(s) you wish to override, not both");

            string baseEvt = e.Args.Get<string> ();
            if (e.Args [0].Value != null) {
                CreateOverride (context, e.Args [0].Get<string> (), new string [] { baseEvt });
            } else {
                foreach (var idxWith in e.Args [0].Children) {
                    CreateOverride (context, idxWith.Get<string> (), new string [] { baseEvt });
                }
            }
        }

        /// <summary>
        /// dynamically removes an existing override
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "delete-override")]
        private static void lambda_delete_override (ApplicationContext context, ActiveEventArgs e)
        {
            string baseActiveEvent = e.Args.Get<string> ();
            if (baseActiveEvent == null) {
                throw new ArgumentException ("[delete-override] requires either an expression, or a constant defining which override to delete");
            }
            if (e.Args.Count == 0) {
                throw new ArgumentException ("[delete-event] requires either an expression, or a constant defining which overrided Active Event you wish to delete");
            }
            string newActiveEvent = e.Args [0].Get<string> ();
            List<string> newActiveEvents = new List<string> ();
            if (Expression.IsExpression (newActiveEvent)) {
                var match = Expression.Create (newActiveEvent).Evaluate (e.Args [0]);
                for (int idxNo = 0; idxNo < match.Count; idxNo ++) {
                    string idxNewAV = match.GetValue (idxNo) as string;
                    if (idxNewAV == null) {
                        throw new ArgumentException ("expression given to [delete-override] yielded a result that was not of type 'string'");
                    }
                    newActiveEvents.Add (idxNewAV);
                }
            } else {
                newActiveEvents.Add (newActiveEvent);
            }
            List<string> baseActiveEvents = new List<string> ();
            if (Expression.IsExpression (baseActiveEvent)) {
                var match = Expression.Create (baseActiveEvent).Evaluate (e.Args);
                for (int idxNo = 0; idxNo < match.Count; idxNo ++) {
                    string overrideToRemove = match.GetValue (idxNo) as string;
                    if (overrideToRemove == null) {
                        throw new ArgumentException ("expression given to [delete-override] yielded a result that was not of type 'string'");
                    }
                    baseActiveEvents.Add (overrideToRemove);
                }
            } else {
                baseActiveEvents.Add (baseActiveEvent);
            }
            foreach (string idxBase in baseActiveEvents) {
                foreach (string idxNew in newActiveEvents) {
                    foreach (var ovr in _overrides) {
                        if (ovr.OverriddenEvent == idxBase && ovr.OverrideEvent == idxNew) {
                            _overrides.Remove (ovr);
                            UnInitializeOverride (context, ovr);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// responsible for "re-mapping" all dynamically created Active Events and overrides. automatically
        /// called by the framework when a new <see cref="phosphorus.core.ApplicationContext"/> is created
        /// and initialized
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
            // retrieving actual event name raised
            string actualEvtName = e.Base.Name;

            // making sure "base event" is attached to lambda execution object, in case 
            // event invokes "call-base", if there is a base event
            if (e.Base.Base != null) {

                // attaching all base Active Events in consecutive order
                e.Args.Insert (0, new Node ("__base"));
                ActiveEventArgs idxArgs = e.Base.Base;
                while (idxArgs != null) {
                    e.Args [0].Add (new Node (string.Empty, idxArgs.Name));
                    idxArgs = idxArgs.Base;
                }

                // invoking event
                InvokeEvent (context, actualEvtName, e.Args);

                // removing "base event" arguments
                e.Args [0].Untie ();
            } else {

                // no reason to massage this bugger
                InvokeEvent (context, actualEvtName, e.Args);
            }
        }

        /*
         * responsible for actually invoking an Active Event from our list of dynamically created events
         */
        private static void InvokeEvent (ApplicationContext context, string actualEvtName, Node args)
        {
            // checking to see if we have an Active Event with the given name
            if (_events.ContainsKey (actualEvtName)) {

                // looping through all dynamically created Active Events with the given name
                foreach (var evt in _events [actualEvtName]) {

                    // creating our lambda execution object, appending all the parameters given to it
                    Node exe = evt.Lambda.Clone ();
                    foreach (Node idxArg in args.Children) {
                        if (idxArg.Name == "__base")
                            exe.Insert (0, idxArg.Clone ());
                        else
                            exe.Add (idxArg.Clone ());
                    }
                    exe.Name = args.Name;
                    exe.Value = args.Value;

                    // actually executing event implementation. no needs to invoke "copy" version, since we've done the dirty
                    // works already copying the event code anyway, and to invoke the [lamba] simple version saves some cycles
                    context.Raise ("lambda", exe);
                }
            }
        }

        /*
         * returns list of overrides
         */
        private static IEnumerable<string> GetOverrides (Node overrideNode)
        {
            if (overrideNode.Value != null && overrideNode.Count > 0)
                throw new ArgumentException ("you cannot declare overrides for [event] both as value and as children, choose one");
            if (overrideNode.Value != null) {
                yield return overrideNode.Get<string> ();
            } else {
                foreach (var idxOverride in overrideNode.Children) {
                    yield return idxOverride.Get<string> ();
                }
            }
        }

        /*
         * creates new Active Event sink
         */
        private static void CreateEvent (ApplicationContext context, string name, Node lambda)
        {
            // creating new dynamic Active Event and adding to our (static) list of events
            Event evt = new Event (name, lambda);
            if (!_events.ContainsKey (name))
                _events [name] = new List<Event> ();
            _events [name].Add (evt);

            // initializing event by creating the correct overrides and such
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
        private static void CreateOverride (ApplicationContext context, string overrideEvent, IEnumerable<string> overrides)
        {
            // looping through all overrides given for event to override, and creating associated overrides as static list
            foreach (var overriddenEvent in overrides) {
                Override over = new Override (overrideEvent, overriddenEvent);
                _overrides.Add (over);

                // actually initializing our override
                InitializeOverride (context, over);
            }
        }

        /*
         * initializes override by creating the necessary mapping on the application context to make sure override is 
         * invoked correctly
         */
        private static void InitializeOverride (ApplicationContext context, Override over)
        {
            context.RemoveOverride (over.OverriddenEvent, "_pf.core.execute-dynamic-lambda");
            context.Override (over.OverriddenEvent, over.OverrideEvent);
        }
        
        /*
         * UN-initializes override by removing any existing mapping on the application context to make sure override is 
         * removed correctly
         */
        private static void UnInitializeOverride (ApplicationContext context, Override over)
        {
            context.RemoveOverride (over.OverriddenEvent, over.OverrideEvent);
            context.Override (over.OverriddenEvent, "_pf.core.execute-dynamic-lambda");
        }
    }
}
