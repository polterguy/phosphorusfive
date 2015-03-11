/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System.Linq;
using System.Text;
using phosphorus.core;
using phosphorus.expressions;
using phosphorus.hyperlisp.helpers;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

/// <summary>
///     Main namespace for parsing and creating Hyperlisp.
/// 
///     Encapsulates all classes and Active Event necessary to create and parse Hyperlisp.
/// 
///     Hyperlisp is the <em>"syntax"</em> that allows you to create pf.lambda execution trees using a textual-based format. Hyperlisp
///     is in fact the only place where actual <em>"parsing"</em> is done in Phosphorus.Five, besides from to some extent Expressions though.
///     This trait completely separates Phosphorus.Five and pf.lambda from all other programming languages, since the syntax for describing
///     your <em>"code"</em>, is completely de-coupled from the actual environment that executes that code.
/// 
///     In fact, if you wish, you can easily create your own XML parser, that parses XML documents, for then to execute the parsed documents
///     as pf.lambda. You could probably even <em>"execute"</em> HTML documents if you wish, or JSON, BSON, or any other document format
///     capable of describing relational tree-structures, including EDIFACT for that matter. However, Hyperlisp is our preferred file-format, 
///     used when writing <em>"code"</em>, that we transform into our pf.lambda execution trees.
/// 
///     Hyperlisp is similar to JSON, or BSON to be more accurate, since it allows you to describe type-information, in addition to JSON's key/value
///     parts. Hyperlisp does however have one unique trait over BSON, which sets it apart from both of the previously mentioned formats, which
///     is its ability to in addition to being a key/value file-format, also explicitly in addition allows you to describe <em>"children nodes"</em>.
///     For instance, imagine a node with a key of <em>"foo"</em>, value of <em>"bar"</em> and two children. In JSON this would have to be described
///     like this;
/// 
///     <pre>{[{"Name" : "foo", "Value" : "bar", "Children": [
///   {
///     "Name" : "foo_child_name1", 
///     "Value" : "foo_child_value1"
///   },
///   {
///     "Name" : "foo_child_name2", 
///     "Value" : "foo_child_value2"
///   }
/// ]}]}</pre>
/// 
/// The above piece of JSON is a LOT of syntax, for very little effect. In Hyperlisp the same structure can be described with this code;
/// 
///     <pre>foo:bar
///   foo_child_name1:foo_child_value1
///   foo_child_name2:foo_child_value2
/// </pre>
/// 
/// We effectively reduced 9 lines of JSON to three lines of Hyperlisp, and while doing so, we reduced the number of control characters from 50 to 3!
///     In addition, if we wanted to communicate that one of our values was not a <em>"string"</em> type, but an integer, we could easily do so by
///     creating a piece of code, resembling something like this;
/// 
///     <pre>foo:bar
///   foo_child_name1:foo_child_value1
///   foo_child_INTEGER:<strong>int</strong>:5
/// </pre>
/// 
/// The above traits with Hyperlisp, makes it superior to describing relational key/value/children tree structures for us, which is why we use 
///     our own invention which we call "Hyperlisp", instead of using an existing file-format, such as JSON or XML.
/// 
///     <em>"One has to be a complete idiot, to not be able to see that there exists better file-formats, for describing relational 
///     tree-structures, than that of JSON"</em> - Douglas Crockford quote, freely re-written ... ;)
/// 
///     The basic structure of Hyperlisp is that it starts with a "key", then there's a colon, followed by the "value". If you have a type,
///     other than string, you'll need to put the type between the "key" and "value". Beneath the node itself, you can describe the node's children
///     nodes, by prepending the names of the children with two spaces (\"&nbsp;&nbsp;\").
/// 
///     The type-system of Hyperlisp is also extendible, which means that you can create your own types, and create support for your own
///     types in Hyperlisp, which you can find examples of in the phosphorus.types project. As long as you can somehow represent your types with
///     a string, you can represent your types in Hyperlisp.
/// 
///     Hyperlisp recognizes strings the same way C# does, which means you can create single-line string literals like this;
/// 
///     <pre>foo:\"this is a single\\nline \\\"string\\\"\"</pre>
/// 
///     And you can create multi-line strings like this;
/// 
///     <pre>foo:\@\"this is a
/// multi line string\"</pre>
/// 
/// Both the name of a node, and its value, can be represented using both single-line string literals, and multi-line string literals. Only
///     the value of a node can have a type associated with it though.
/// 
///     To create a comment in Hyperlisp, is similar to creating comments in C#, except for the fact, that a comment cannot appear in any ways
///     on the same line as a semantic key/value portion of your Hyperlisp. Comments are completely ignored, and will not in any ways become a
///     part of your pf.lambda structure. This is done to make sure you can comment your code, without running the risk that your comment starts
///     adding to your bandwidth usage when transmitting pf.lambda code over the wire, using for instance HTTP. Below is an example of a 
///     single-line comment.
/// 
///     <pre>// Single line comment
/// foo:bar
/// // Another single line comment</pre>
/// 
/// Whitespace characters, are as a general rule ignored, except in front of the "name" portion. When you declare children nodes of a node,
///     you do so by putting the child node on a line underneath the node it is supposed to be a child node of, and add two additional spaces
///     in front of the name. This means that if the name of your node, starts with a whitespace character, then you must put the name of your
///     node inside some sort of string literal. Consider this;
/// 
///     <pre>foo:bar
///   " foo2 ":\"&nbsp;&nbsp;&nbsp;White spaces are NOT ignored here!&nbsp;&nbsp;&nbsp;\"
///   foo3&nbsp;&nbsp;&nbsp;:&nbsp;&nbsp;&nbsp;While this value, and name, is basically 'trimmed' ...&nbsp;&nbsp;&nbsp;
///  &nbsp;&nbsp;
///     // The line above this line, and below, is completely ignored by the Hyperlisp parser
///  &nbsp;&nbsp;
///     foo4:While this is actually a child of the 'foo3' node!</pre>
/// 
///     By using multi-line string literals as the names of your nodes, it is very much possible to almost completely destroy the readability
///     of your Hyperlisp, therefor we suggest that you use the names portion of your nodes, mostly for things that don't require carriage returns, 
///     or white-space characters in the beginning or end of your names.
/// 
///     You can also nest string literals inside of each other, consider the following code;
/// 
///     <pre>_hyperlisp:node:\@\"_data
///   foo:\@\"\"Multi line
/// string literal, which is a part of
/// a reference node\"\"\"</pre>
/// 
/// If you nest too many levels of string literals inside string literals, then obviously your code will rapidly become completely impossible to
///     understand, so be careful with these features of the language.
/// 
/// The rules for string literals, are the exact same rules as for C#. You can use any of the escape characters you're used to from C# in
///     single-line string literals. Multi-line string literals also works identical to how they work in C#.
/// 
///     Below is an example of a deep hierarchy of nodes, some of these nodes contains Hyperlisp syntactic errors though;
/// 
///     <pre>_root1
///   foo1:bar1
///     foo2:I am a child of foo1
///     foo3:I am also a child of foo1
///       foo4:While I am a child of foo3
///     foo5:While I am a child of foo1 again
/// _root2:I am another root node
///    foo6:I am a 'syntax error', with three spaces in front of my name. To fix me, remove one space!
/// _root2:Same name as my 'elder brother', which is perfectly valid Hyperlisp!
///     foo7:\@\"I am a 'syntax error', I apparently seem to be the grandchild of _root2, 
/// without any children of _root2 existing. To 'fix me', remove two of the spaces in front of my 'name' portion!\"
/// _root3:I am a syntax error, since I contain a ':' character inside of my value! To fix me, double-quote me!</pre>
/// 
/// Hyperlisp is not a replacement for neither JSON nor BSON, and in fact, Phosphorus.Ajax uses for instance JSON as its Ajax
///     transport mechanism, when returning values from the server. But for declaring key/value/children tree-structures on the server-side,
///     it is very much superior to both JSON and XML. First of all, since it significantly reduces the size of its files compared to both JSON
///     and XML. Secondly, because it provides much more intuitive and easily understood syntax. Thirdly, because it provides an extendible
///     type mechanism, allowing you to store type-information together with your data.
/// 
///     When returning objects from your server to a web-browser client, you should probably stay with JSON, but when sending values between
///     servers, and storing pf.lambda execution trees, then Hyperlisp is superior in all ways. This can be seen by the fact that the phosphorus.data
///     database, in its entirety, uses Hyperlisp as its file format. Hyperlisp is also probably superior in all ways compared to XML when it
///     comes to the concept of web-services, and sending data back and forth between what you'd normally use an XML Web Service for. Among other
///     things, because of its significantly smaller size, than that of XML.
/// 
///     \htmlonly<iframe width="560" height="315" src="https://www.youtube.com/embed/yptjGVYOhu4" frameborder="0" allowfullscreen></iframe>\endhtmlonly
/// 
/// Hyperlisp is also extremely convenient when it comes to storing pf.lambda objects in files on disc. To load a Hyperlisp file, transform it
///     to pf.lambda, for then to execute the transformed pf.lambda object, can be done with three lines of code;
/// 
///     <pre>pf.file.load:foo.hl
/// pf.hyperlisp.hyperlisp2lambda:@/-/0?value
/// lambda.copy:@/-?node</pre>
/// </summary>
namespace phosphorus.hyperlisp
{
    /// <summary>
    ///     Class to help transform between Hyperlisp and <see cref="phosphorus.core.Node">Nodes</see>.
    /// 
    ///     Contains the [pf.hyperlisp.hyperlisp2lambda] and the [pf.hyperlisp.lambda2hyperlisp] Active Events,
    ///     necessary to be able to parse Hyperlisp from pf.lambda nodes, and vice versa.
    /// </summary>
    public static class PFHyperlisp
    {
        /// <summary>
        ///     Tranforms the given Hyperlisp to a pf.lambda node structure.
        /// 
        ///     Active Event will transform the given Hyperlisp to a pf.lambda node structure.
        /// 
        ///     Example;
        /// 
        ///     <pre>pf.hyperlisp.hyperlisp2lambda:\@"foo:bar
        ///   foo-child-1:"" Howdy World! ""
        ///   foo-child-2:int:234
        ///   foo-date:date:2014-05-16"</pre>
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "pf.hyperlisp.hyperlisp2lambda")]
        private static void pf_hyperlisp_hyperlisp2lambda (ApplicationContext context, ActiveEventArgs e)
        {
            var builder = new StringBuilder ();
            foreach (var idx in XUtil.Iterate<string> (e.Args, context)) {
                builder.Append (idx + "\r\n");
            }
            e.Args.AddRange (new NodeBuilder (context, builder.ToString ().TrimEnd ('\r', '\n', ' ')).Nodes);
        }

        /// <summary>
        ///     Transforms the given pf.lambda node structure to Hyperlisp.
        /// 
        ///     Active Event will transform the given pf.lambda node structure to Hyperlisp.
        /// 
        ///     Example;
        /// <pre>_data
        ///   foo1:bar1
        ///   foo2:bar2
        /// pf.hyperlisp.lambda2hyperlisp:@/-?node</pre>
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.hyperlisp.lambda2hyperlisp")]
        private static void pf_code_lambda2hyperlisp (ApplicationContext context, ActiveEventArgs e)
        {
            if (XUtil.IsExpression (e.Args.Value)) {
                var nodeList = XUtil.Iterate<Node> (e.Args, context).ToList ();
                e.Args.Value = new HyperlispBuilder (context, nodeList).Hyperlisp;
            } else {
                var node = e.Args.Value as Node;
                e.Args.Value = node != null ? new HyperlispBuilder (context, new[] {node}).Hyperlisp : new HyperlispBuilder (context, e.Args.Children).Hyperlisp;
            }
        }
    }
}
