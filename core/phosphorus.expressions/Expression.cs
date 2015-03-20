/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Globalization;
using phosphorus.core;
using phosphorus.expressions.exceptions;
using phosphorus.expressions.iterators;

// ReSharper disable UnusedParameter.Local

/// <summary>
///     Main namespace for the Expression engine in Phosphorus.Five.
/// 
///     This namespace contains the Expression engine for Phosphorus.Five, and is what allows you to compose
///     expressions, extracting node result-set from your pf.lambda execution tree.
/// </summary>
namespace phosphorus.expressions
{
    /// <summary>
    ///     The main expression class in Phosphorus.Five.
    /// 
    ///     Responsible for parsing, building, and evaluating your pf.lambda expressions, according to what types of iterators
    ///     you mix together, to form your complete expression.
    /// 
    ///     An expression is normally recognized automatically by any Active Events that supports them, and starts with a "@" character,
    ///     followed by any number of <see cref="phosphorus.expressions.iterators.Iterator">Iterators</see>, ending with a type declaration.
    /// 
    ///     Example;
    /// 
    ///     <pre>@/../"*"?node</pre>
    /// 
    ///     The above example will find all children nodes of the root node of your current execution tree.
    /// 
    ///     There are 5 different types of expressions you can declare;
    /// 
    ///     1. <strong>?value</strong> - Returns the <see cref="phosphorus.core.Node.Value">value</see> property of your nodes.
    ///     2. <strong>?name</strong> - Returns the <see cref="phosphorus.core.Node.Name">name</see> property of your nodes.
    ///     3. <strong>?count</strong> - Returns the <see cref="phosphorus.core.Node.Count">count</see> property of your nodes.
    ///     4. <strong>?path</strong> - Returns the <see cref="phosphorus.core.Node.Path">path</see> property of your nodes.
    ///     5. <strong>?node</strong> - Returns the actual <see cref="phosphorus.core.Node">nodes</see> themselves.
    /// 
    ///     Both '?count' and '?path' types of expressions are "read-only", and cannot be used as destinations, or changed in any
    ///     ways. All other types of expressions, can be both assigned to, and retrieved.
    /// 
    ///     Each iterator, starts with a forward slash /, and ends with any of the following characters; "!&^|()?/". If you wish to
    ///     use any of the previously mentioned 8 special characters as parts of your iterators, you must put your entire iterator inside
    ///     double-quotes, for instance;
    /// 
    ///     <pre>@/"foo&bar"?value</pre>
    /// 
    ///     The above expression will look for a node who's name is "foo&bar".
    /// 
    ///     You can use any number of iterators when you compose your expressions, and each iterator will be evaluated in a left-to-right manner,
    ///     which means that your first iterator will be evaluated first, and then used as the input to your next iterator. This makes your
    ///     iterators becomes evaluated as a "chain of enumerables", where each iterator appends a new "filter criteria", and creates a new
    ///     result, based upon its previous iterator, that it passes into the next iterator in the chain.
    /// 
    ///     It might help to think of expressions as XPath expressions, since they to some extent do the same thing.
    /// 
    ///     By mixing the different <see cref="phosphorus.expressions.iterators.Iterator">Iterators</see> in your expressions, together
    ///     with <see cref="phosphorus.expressions.Logical">Logicals</see>, allowing for you to perform Boolean Algebraic operations
    ///     on expressions, and sub-expressions, you can probably extract any node-set you wish, from any starting pf.lambda node tree 
    ///     you wish to create a sub-set from. This is why it is a <em>"Hyperdimensional Boolean Algebraic graph object Expression 
    ///     implementation"</em>. Since it allows you to treat graph objects, as if they have an infinite number of additional dimensions, in
    ///     addition to the two that are automatically given in all graph objects, which are; width and height.
    /// 
    ///     Below is an example of an expression that will extract all 'names', from all nodes from the root of your tree, having a parent 
    ///     node, who's value equals "foo", but who's name is not "bar", but only if the node has at least one children node of its own.
    /// 
    ///     <pre>@/../"**"(/=foo!/bar&/0/.)?name</pre>
    /// 
    ///     To understand the powers of pf.lambda expressions might be difficult when you start using Phosphorus.Five. However, it might
    ///     help to think of them as an alternative to algorithms, or a dynamica version of LINQ from C#, or stored IEnumerables, if you wish.
    ///     But if you can imagine a result you wish to extract from a pf.lambda node tree, you can probably create an expression that can extract
    ///     that node-set for you. For instance, imagine trying to set the value of all intelligent animals main value node to "yes", except
    ///     Homo-Sapien, in the below data structure;
    /// 
    ///     <pre>_data
    ///   homo-sapien
    ///     iq:yes
    ///   ape
    ///     iq:yes
    ///   donkey
    ///     iq:no
    ///   dog
    ///     iq:no
    ///   fish-like-mammals
    ///     dolphins
    ///       iq:yes
    ///     salmon
    ///       iq:no
    ///     killer-whales
    ///       iq:yes
    /// set:@/../"**"/iq/=yes/.(!/homo-sapiens)?value
    ///   source:yes</pre>
    /// 
    ///     Creating C# code to do the equivalent of the above pf.lambda expression, would end up in a monstrous, recursive, multiple methods 
    ///     implementation, spanning possibly dozens, if not more than 100 lines of code. With pf.lambda expressions, it's a simple one-liner 
    ///     expression, roughly 40 characters long.
    /// 
    ///     With pf.lambda expressions, you can very often completely eliminate recursive method invocations, and in fact even conditional branching,
    ///     and methods in general, by intelligently composing your expressions together, to extract what ever result-set you want to manipulate,
    ///     or retrieve. With pf.lambda expressions, you can easily for instance create a list result-set, from a relational graph object, without
    ///     resorting to recursive methods, or similar concepts. Consider for instance the code below, that creates a list of all the intelligent
    ///     and sentient species on our planets;
    /// 
    ///     <pre>_destination
    /// _data
    ///   binary-based-life-forms
    ///     terminator-t1
    ///       iq:no
    ///     World-Wide-Web
    ///       iq:yes
    ///   homo-sapien
    ///     iq:yes
    ///   ape
    ///     iq:yes
    ///   donkey
    ///     iq:no
    ///   dog
    ///     iq:no
    ///   fish-like-mammals
    ///     dolphins
    ///       iq:yes
    ///     salmon
    ///       iq:no
    ///     killer-whales
    ///       iq:yes
    /// append:@/-2?node
    ///   source:@/../"*"/_data/"**"/iq/=yes/.?name</pre>
    /// 
    ///     You can also convert the results of your expressions, to any of the types that there exists a [pf.hyperlisp.get-object-value.xxx]
    ///     Active Event for. You do this by appending the type after your expression's type declaration, separated with a period (.).
    ///     For instance, to convert the results of an expression to integers, you could use something like this;
    /// 
    ///     <pre>_input:5
    /// _result
    /// set:@/-?value
    ///   source:@/./-2?value.int</pre>
    /// 
    ///     You can also create <em>"reference expressions"</em>, which are expressions leading to other expressions, where you do not want to
    ///     retrieve the expressions your expression is leading too, but rather what those expressions are referring to. When you create
    ///     reference expressions, then the results from your outer expressions will be evaluated, and if the results are another expression,
    ///     then that expression will be evaluated, and its results returned back to the original caller. This way you can combine constant
    ///     values and expressions, and create reference expressions, that will either return the constant they point to, or the value their
    ///     referenced expressions points to. Consider this;
    /// 
    ///     <pre>_src:su
    ///   first:@/.?value
    ///   second:cc
    ///   third:@/+?name
    ///   ess
    /// _destination
    /// set:@/-?value
    ///   source:\@@/./-2/"*"?value</pre>
    /// 
    ///     Most Active Events in Phosphorus.Five can take expressions as their arguments. Understanding expressions, is key to learning
    ///     Phosphorus.Five.
    /// 
    ///     PS!<br/>
    ///     Normally you don't want to consume this class directly, but instead use it indirectly through the XUtil class,
    ///     which contains many helper methods to create and evaluate expressions for you!
    /// 
    ///     PPS!<br/>
    ///     You normally don't need to put neither the "*" iterator, nor the "**" inside of double-quotes, but since the documentation
    ///     system we're using to create this manual, recognizes a slash (/) followed by an asterix (*) as a "special character", all
    ///     places where we use the "*" iterator, and the "**" iterator, must be escaped inside of double-quotes for the documentation
    ///     of Phosphorus.Five. This is not necessary when you create your own expressions, but a restriction of our documentation generator.
    /// </summary>
    public class Expression
    {
        // contains actual expression we're evaluating
        private readonly string _expression;
        private ApplicationContext _context;
        // these next two buggers are kept around to provide contextual information for exceptions,
        // among other things, and to make conversions possible
        private Node _evaluatedNode;

        /*
         * private ctor, to make sure we can extend creation logic in the future
         */
        private Expression (string expression)
        {
            // ps, postponing syntax checking until "Evaluate", since we've got "context" in Evaluate
            _expression = expression;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="phosphorus.expressions.Expression" /> class.
        /// </summary>
        /// <param name="expression">\Expression to evaluate</param>
        public static Expression Create (string expression)
        {
            return new Expression (expression);
        }

        /// <summary>
        ///     Evaluates expression for given <see cref="phosphorus.core.Node">node</see>.
        /// 
        ///     Returns a match object wrapping the result from your Expression.
        /// 
        ///     PS!
        ///     Normally you very seldom want to use this method directly, or this class for that matter, but instead
        ///     use one of the helper methods in the XUtil class, which takes care of creating expressions, and evaluating
        ///     then automatically for you.
        /// </summary>
        public Match Evaluate (Node node, ApplicationContext context)
        {
            // verifying we've got an actual expression, since expression should be finished formatted
            // at this point, we can use "Exact" version
            if (!XUtil.IsExpression (_expression))
                throw new ExpressionException (_expression, node, context);

            // storing these bugger for later references, used in exceptions, among other things
            _evaluatedNode = node;
            _context = context;

            // creating our "root group iterator"
            var current = new IteratorGroup (node);
            string typeOfExpression = null, previousToken = null;

            // Tokenizer uses StringReader to tokenize, making sure tokenizer is disposed when finished
            using (var tokenizer = new Tokenizer (_expression)) {
                // looping through every token in espression, building up our Iterator tree hierarchy
                foreach (var idxToken in tokenizer.Tokens) {
                    if (previousToken == "?") {
                        // this is our last token, storing it as "expression type", before ending iteration
                        typeOfExpression = idxToken;
                        break;
                    }
                    if (idxToken != "?") {
                        // ignoring "?", handled in next iteration

                        // building expression tree
                        current = AppendToken (current, idxToken, previousToken);
                    }

                    // storing previous token, since some iterators are dependent upon knowing it
                    previousToken = idxToken;
                }
            }

            // creating a Match object, and returning to caller
            return CreateMatchFromIterator (current, typeOfExpression);
        }

        /*
         * handles an expression iterator token
         */
        private IteratorGroup AppendToken (
            IteratorGroup current,
            string token,
            string previousToken)
        {
            switch (token) {
                case "(":

                    // opening new group
                    return new IteratorGroup (current);
                case ")":

                    // closing group
                    if (current.ParentGroup == null) // making sure there's actually an open group first
                        throw new ExpressionException (
                            _expression,
                            "Closing parenthesis ')' has no matching '(' in expression.",
                            _evaluatedNode,
                            _context);
                    return current.ParentGroup;
                case "/":

                    // new token iterator
                    if (previousToken == "/") {
                        // two slashes "//" preceding each other, hence we're looking for a named value,
                        // where its name is string.Empty
                        current.AddIterator (new IteratorNamed (string.Empty));
                    }
                    break;
                case "|":
                case "&":
                case "^":
                case "!":

                    // boolean algebraic operator, opening up a new sub-expression
                    LogicalToken (current, token, previousToken);
                    break;
                case "@":

                    // reference expression, but only if it is the first token in expression
                    if (previousToken != null) {
                        DefaultToken (current, token, previousToken, _context);
                    } else if (current.IsReference) {
// making sure reference expressions can only be declared once
                        throw new ExpressionException (
                            _expression,
                            "You cannot declare your expression to be a reference expression more than once.",
                            _evaluatedNode,
                            _context);
                    }
                    current.IsReference = true;
                    break;
                case "..":

                    // root node token
                    current.AddIterator (new IteratorRoot ());
                    break;
                case "*":

                    // all children token
                    current.AddIterator (new IteratorChildren ());
                    break;
                case "**":

                    // flatten descendants token
                    current.AddIterator (new IteratorFlatten ());
                    break;
                case ".":

                    // parent node token
                    current.AddIterator (new IteratorParent ());
                    break;
                case "#":

                    // reference node token
                    current.AddIterator (new IteratorReference (_context));
                    break;
                case "<":

                    // left shift token
                    current.AddIterator (new IteratorShiftLeft ());
                    break;
                case ">":

                    // right shift token
                    current.AddIterator (new IteratorShiftRight ());
                    break;
                default:

                    // handles everything else
                    DefaultToken (current, token, previousToken, _context);
                    break;
            }

            // defaulting to returning what we came in with
            return current;
        }

        /*
         * handles "|", "&", "!" and "^" tokens
         */
        private static void LogicalToken (IteratorGroup current, string token, string previousToken)
        {
            switch (token) {
                case "|":

                    // OR logical boolean algebraic operator
                    current.AddLogical (new Logical (Logical.LogicalType.Or));
                    break;
                case "&":

                    // AND logical boolean algebraic operator
                    current.AddLogical (new Logical (Logical.LogicalType.And));
                    break;
                case "!":

                    // NOT logical boolean algebraic operator
                    current.AddLogical (new Logical (Logical.LogicalType.Not));
                    break;
                case "^":

                    // XOR logical boolean algebraic operator
                    current.AddLogical (new Logical (Logical.LogicalType.Xor));
                    break;
            }
        }

        /*
         * handles all other tokens, such as "named tokens" and "valued tokens"
         */
        private void DefaultToken (
            IteratorGroup current,
            string token,
            string previousToken,
            ApplicationContext context)
        {
            if (token.StartsWith ("=")) {
                // some type of value token, either normal value, or regex value
                ValueToken (current, token);
            } else if (token.StartsWith ("[")) {
                // range iterator token
                RangeToken (current, token);
            } else if (token.StartsWith ("..") && token.Length > 2) {
                // named ancestor token
                current.AddIterator (new IteratorNamedAncestor (token.Substring (2)));
            } else if (token.StartsWith ("%")) {
                // modulo token
                ModuloToken (current, token);
            } else if (token.StartsWith ("-") || token.StartsWith ("+")) {
                // modulo token
                SiblingToken (current, token);
            } else if (token.StartsWith ("/")) {
                // named regex token
                current.AddIterator (new IteratorNamedRegex (token, _expression, _evaluatedNode, _context));
            } else {
                if (Utilities.IsNumber (token)) {
                    // numbered child token
                    current.AddIterator (new IteratorNumbered (int.Parse (token)));
                } else {
                    // defaulting to "named iterator"
                    current.AddIterator (new IteratorNamed (token));
                }
            }
        }

        /*
         * value token, either a regular expression token, or a normal value comparison token,
         * optionally with a type declaration
         */
        private void ValueToken (IteratorGroup current, string token)
        {
            if (token.IndexOf ('/') == 1) {
                // value token, with regular expression
                ValueTokenRegex (current, token);
            } else {
                // value token, not regex, possibly a type declaration though
                ValueTokenNormal (current, token);
            }
        }

        /*
         * creates a valued regex token
         */
        private void ValueTokenRegex (IteratorGroup current, string token)
        {
            token = token.Substring (1); // removing equal sign (=)
            current.AddIterator (new IteratorValuedRegex (token, _expression, _evaluatedNode, _context));
        }

        /*
         * creates a valued token
         */
        private void ValueTokenNormal (IteratorGroup current, string token)
        {
            token = token.Substring (1); // removing equal sign (=)
            string type = null; // defaulting to "no type", meaning "string" type basically
            if (token.IndexOf ('\\') == 0) {
                // escaped equality token, necessary to support ":" as beginning of string values,
                // without having type information tranformation logic kicking in
                token = token.Substring (1);
            } else {
                // might contain a type declaration, checking here
                if (token.IndexOf (':') == 0) {
                    // yup, we've got a type declaration for our token ...
                    type = token.Substring (1, token.IndexOf (":", 1, StringComparison.Ordinal) - 1);
                    token = token.Substring (type.Length + 2);
                }
            }
            current.AddIterator (new IteratorValued (token, type, _context));
        }

        /// \todo cleanup, too long ...
        /*
         * creates a range token [x,y]
         */
        private void RangeToken (IteratorGroup current, string token)
        {
            // verifying token ends with "]"
            if (token [token.Length - 1] != ']')
                throw new ExpressionException (
                    _expression,
                    string.Format ("Syntax error in range token '{0}', no ']' at end of token", token),
                    _evaluatedNode,
                    _context);

            if (token.IndexOf (',') != -1) {
                token = token.Substring (1, token.Length - 2);
                var values = token.Split (',');

                // verifying token has only two integer values, separated by ","
                if (values.Length != 2)
                    throw new ExpressionException (
                        _expression,
                        string.Format ("Syntax error in range token '[{0}]', ranged iterator takes two integer values, separated by ','", token),
                        _evaluatedNode,
                        _context);
                var start = -1;
                var end = -1;
                var startStr = values [0].Trim ();
                var endStr = values [1].Trim ();
                if (startStr.Length > 0) {
                    if (!Utilities.IsNumber (startStr))
                        throw new ExpressionException (
                            _expression,
                            string.Format ("Syntax error in range token '[{0}]', expected number, found string", token),
                            _evaluatedNode,
                            _context);
                    start = int.Parse (startStr, CultureInfo.InvariantCulture);
                }
                if (endStr.Length > 0) {
                    if (!Utilities.IsNumber (endStr))
                        throw new ExpressionException (
                            _expression,
                            string.Format ("Syntax error in range token '[{0}]', expected number, found string", token),
                            _evaluatedNode,
                            _context);
                    end = int.Parse (endStr, CultureInfo.InvariantCulture);
                    if (end <= start)
                        throw new ExpressionException (
                            _expression,
                            string.Format ("Syntax error in range token '[{0}]', end must be larger than start", token),
                            _evaluatedNode,
                            _context);
                }
                current.AddIterator (new IteratorRange (start, end));
            } else {
                throw new ExpressionException (
                    _expression,
                    string.Format ("Syntax error in range token '{0}', expected two values, found one", token),
                    _evaluatedNode,
                    _context);
            }
        }

        /*
         * creates a modulo token
         */
        private void ModuloToken (IteratorGroup current, string token)
        {
            // removing "%" character
            token = token.Substring (1);

            // making sure we're given a number
            if (!Utilities.IsNumber (token))
                throw new ExpressionException (
                    _expression,
                    string.Format ("Syntax error in modulo token '{0}', expected integer value, found string", token),
                    _evaluatedNode,
                    _context);
            current.AddIterator (new IteratorModulo (int.Parse (token)));
        }

        /*
         * creates a sibling token
         */
        private void SiblingToken (IteratorGroup current, string token)
        {
            var intValue = token.Substring (1);
            var oper = token [0];
            var value = 1;
            if (intValue.Length > 0 && !Utilities.IsNumber (intValue))
                throw new ExpressionException (
                    _expression,
                    string.Format ("Syntax error in sibling token '{0}', expected integer value, found string", token),
                    _evaluatedNode,
                    _context);
            if (intValue.Length > 0)
                value = int.Parse (intValue);
            current.AddIterator (new IteratorSibling (value*(oper == '+' ? 1 : -1)));
        }

        /*
         * create a Match object from an Iterator group
         */
        private Match CreateMatchFromIterator (IteratorGroup group, string type)
        {
            // checking to see if we have open groups, which is an error
            if (group.ParentGroup != null)
                throw new ExpressionException (_expression, "Group in expression was not closed.", _evaluatedNode, _context);

            // parsing type of match
            /// \todo shares a lot of functionality with XUtil.ExpressionType, try to refactor
            string convert = null;
            if (type.Contains (".")) {
                convert = type.Substring (type.IndexOf ('.') + 1);
                type = type.Substring (0, type.IndexOf ('.'));
            }
            Match.MatchType matchType;
            switch (type) {
                case "node":
                case "value":
                case "count":
                case "name":
                case "path":
                    matchType = (Match.MatchType) Enum.Parse (typeof (Match.MatchType), type);
                    break;
                default:
                    throw new ExpressionException (
                        _expression,
                        string.Format ("'{0}' is an unknown type declaration for your expression", type),
                        _evaluatedNode,
                        _context);
            }

            // checking if expression is a reference expression, 
            // at which point we'll have to evaluate all referenced expressions
            if (group.IsReference) {
                // expression is a "reference expression", 
                // meaning we'll have to evaluate all referenced expressions
                var match = new Match (group.Evaluate, matchType, _context, convert);
                return EvaluateReferenceExpression (match, _context, convert);
            }
            // returning simple match object
            return new Match (@group.Evaluate, matchType, _context, convert);
        }

        /*
         * evaluates a reference expression
         */
        private Match EvaluateReferenceExpression (Match match, ApplicationContext context, string convert)
        {
            // making sure only 'value' and 'name' expression types can be "reference expressions"
            if (match.TypeOfMatch != Match.MatchType.name && match.TypeOfMatch != Match.MatchType.value) {
                // logical error, since only 'value' and 'name' expressions can possibly do something intelligent
                // as reference expressions
                throw new ExpressionException (
                    _expression,
                    "Only 'value' and 'name' expressions can be reference expressions",
                    _evaluatedNode,
                    context);
            }

            // looping through referenced expressions, yielding result from these referenced expression(s)
            var retVal = new Match (match.TypeOfMatch, context, convert);

            // looping through each match from reference expression
            foreach (var idxMatch in match) {
                // evaluating referenced expressions, but only if they actually contain expressions
                if (!XUtil.IsExpression (idxMatch.Value)) {
                    // current MatchEntity is not an expression, adding entity as it is
                    retVal.Entities.Add (idxMatch);
                } else {
                    // current MatchEntity contains an expression as its value, evaluating expression, and
                    // adding result of expression
                    /// \todo support formatting expressions through delegate callbacks, such that XUtil.Iterate
                    // and similar constructs can handle reference expressions, through callback, where referenced
                    // expression contains formatting parameters
                    var innerMatch = Create (Utilities.Convert<string> (idxMatch.Value, context)).Evaluate (idxMatch.Node, context);
                    foreach (var idxInner in innerMatch) {
                        retVal.Entities.Add (idxInner);
                    }
                }
            }

            // returning new Match, created by evaluating given "match" parameter
            return retVal;
        }
    }
}
