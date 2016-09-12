/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Numerics;
using p5.exp;
using p5.core;

/// <summary>
///     Main snamespace for all math Active Events
/// </summary>
namespace p5.math
{
    /// <summary>
    ///     Class wrapping core math functions
    /// </summary>
    public static class MathFunctions
    {
        /// <summary>
        ///     Returns absolute value of given constant or expression
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "abs", Protection = EventProtection.LambdaClosed)]
        public static void abs (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up after ourselves
            using (new Utilities.ArgsRemover (e.Args)) {

                // Returning absolute value of given constant or expression
                dynamic value = XUtil.Single<dynamic> (context, e.Args, true);

                // Checking if value is BigInteger, at which case we cannot use System.Math
                if (value is BigInteger) {
                    e.Args.Value = BigInteger.Abs (value);
                } else {
                    e.Args.Value = Math.Abs (value);
                }
            }
        }

        /// <summary>
        ///     Returns ACos value of given constant or expression
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "acos", Protection = EventProtection.LambdaClosed)]
        public static void acos (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up after ourselves
            using (new Utilities.ArgsRemover (e.Args)) {

                // Returning absolute value of given constant or expression
                double value = XUtil.Single<double> (context, e.Args, true);

                // Returning ACos of given value
                e.Args.Value = Math.Acos (value);
            }
        }

        /// <summary>
        ///     Returns ASin value of given constant or expression
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "asin", Protection = EventProtection.LambdaClosed)]
        public static void asin (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up after ourselves
            using (new Utilities.ArgsRemover (e.Args)) {

                // Returning absolute value of given constant or expression
                double value = XUtil.Single<double> (context, e.Args, true);

                // Returning ASin of given value
                e.Args.Value = Math.Asin (value);
            }
        }

        /// <summary>
        ///     Returns ATan value of given constant or expression
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "atan", Protection = EventProtection.LambdaClosed)]
        public static void atan (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up after ourselves
            using (new Utilities.ArgsRemover (e.Args)) {

                // Returning absolute value of given constant or expression
                double value = XUtil.Single<double> (context, e.Args, true);

                // Returning ATan of given value
                e.Args.Value = Math.Atan (value);
            }
        }

        /// <summary>
        ///     Returns ceiling value of given constant or expression
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "ceiling", Protection = EventProtection.LambdaClosed)]
        public static void ceiling (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up after ourselves
            using (new Utilities.ArgsRemover (e.Args)) {

                // Returning absolute value of given constant or expression
                double value = XUtil.Single<double> (context, e.Args, true);

                // Returning ceiling of given value
                e.Args.Value = Math.Ceiling (value);
            }
        }

        /// <summary>
        ///     Returns floor value of given constant or expression
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "floor", Protection = EventProtection.LambdaClosed)]
        public static void floor (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up after ourselves
            using (new Utilities.ArgsRemover (e.Args)) {

                // Returning absolute value of given constant or expression
                double value = XUtil.Single<double> (context, e.Args, true);

                // Returning ceiling of given value
                e.Args.Value = Math.Floor (value);
            }
        }

        /// <summary>
        ///     Returns cos value of given constant or expression
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "cos", Protection = EventProtection.LambdaClosed)]
        public static void cos (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up after ourselves
            using (new Utilities.ArgsRemover (e.Args)) {

                // Returning absolute value of given constant or expression
                double value = XUtil.Single<double> (context, e.Args, true);

                // Returning ceiling of given value
                e.Args.Value = Math.Cos (value);
            }
        }

        /// <summary>
        ///     Returns hyperbolic cos value of given constant or expression
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "cosh", Protection = EventProtection.LambdaClosed)]
        public static void cosh (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up after ourselves
            using (new Utilities.ArgsRemover (e.Args)) {

                // Returning absolute value of given constant or expression
                double value = XUtil.Single<double> (context, e.Args, true);

                // Returning ceiling of given value
                e.Args.Value = Math.Cosh (value);
            }
        }

        /// <summary>
        ///     Returns log value of given constant or expression
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "log", Protection = EventProtection.LambdaClosed)]
        public static void log (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up after ourselves
            using (new Utilities.ArgsRemover (e.Args)) {

                // Returning absolute value of given constant or expression
                dynamic value = XUtil.Single<dynamic> (context, e.Args, true);

                // Checking if value is BigInteger, at which case we cannot use System.Math
                if (value is BigInteger) {
                    e.Args.Value = BigInteger.Log (value);
                } else {
                    e.Args.Value = Math.Log (value);
                }
            }
        }

        /// <summary>
        ///     Returns log 10 value of given constant or expression
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "log10", Protection = EventProtection.LambdaClosed)]
        public static void log10 (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up after ourselves
            using (new Utilities.ArgsRemover (e.Args)) {

                // Returning absolute value of given constant or expression
                dynamic value = XUtil.Single<dynamic> (context, e.Args, true);

                // Checking if value is BigInteger, at which case we cannot use System.Math
                if (value is BigInteger) {
                    e.Args.Value = BigInteger.Log10 (value);
                } else {
                    e.Args.Value = Math.Log10 (value);
                }
            }
        }

        /// <summary>
        ///     Returns rounded value of given constant or expression
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "round", Protection = EventProtection.LambdaClosed)]
        public static void round (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up after ourselves
            using (new Utilities.ArgsRemover (e.Args)) {

                // Returning absolute value of given constant or expression
                double value = XUtil.Single<double> (context, e.Args, true);

                // Returning ceiling of given value
                e.Args.Value = Math.Round (value);
            }
        }

        /// <summary>
        ///     Returns sin value of given constant or expression
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "sin", Protection = EventProtection.LambdaClosed)]
        public static void sin (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up after ourselves
            using (new Utilities.ArgsRemover (e.Args)) {

                // Returning absolute value of given constant or expression
                double value = XUtil.Single<double> (context, e.Args, true);

                // Returning ceiling of given value
                e.Args.Value = Math.Sin (value);
            }
        }

        /// <summary>
        ///     Returns hyperbolic sin value of given constant or expression
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "sinh", Protection = EventProtection.LambdaClosed)]
        public static void sinh (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up after ourselves
            using (new Utilities.ArgsRemover (e.Args)) {

                // Returning absolute value of given constant or expression
                double value = XUtil.Single<double> (context, e.Args, true);

                // Returning ceiling of given value
                e.Args.Value = Math.Sinh (value);
            }
        }

        /// <summary>
        ///     Returns square root value of given constant or expression
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "sqrt", Protection = EventProtection.LambdaClosed)]
        public static void sqrt (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up after ourselves
            using (new Utilities.ArgsRemover (e.Args)) {

                // Returning absolute value of given constant or expression
                double value = XUtil.Single<double> (context, e.Args, true);

                // Returning ceiling of given value
                e.Args.Value = Math.Sqrt (value);
            }
        }

        /// <summary>
        ///     Returns tan value of given constant or expression
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "tan", Protection = EventProtection.LambdaClosed)]
        public static void tan (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up after ourselves
            using (new Utilities.ArgsRemover (e.Args)) {

                // Returning absolute value of given constant or expression
                double value = XUtil.Single<double> (context, e.Args, true);

                // Returning ceiling of given value
                e.Args.Value = Math.Tan (value);
            }
        }

        /// <summary>
        ///     Returns hyperbolic tan value of given constant or expression
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "tanh", Protection = EventProtection.LambdaClosed)]
        public static void tanh (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up after ourselves
            using (new Utilities.ArgsRemover (e.Args)) {

                // Returning absolute value of given constant or expression
                double value = XUtil.Single<double> (context, e.Args, true);

                // Returning ceiling of given value
                e.Args.Value = Math.Tanh (value);
            }
        }
    }
}
