using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace InfixToPostfix
{
    /// <summary>
    /// Main interface of the program.
    /// </summary>
    public partial class Form1 : Form
    {
        //////////////////////////////////////////// Class Declaration //////////////////////////////////////////

        /// <summary>
        /// Stores the constant values.
        /// </summary>
        private static class Constant
        {
            /// <summary>
            /// Contains priorities for accepted operators.
            /// </summary>
            public static readonly Dictionary<string, Operator> operatorPriority = 
                new Dictionary<string, Operator>()
                {
                    ["<"] = new Operator("<", 3, Associativity.left),
                    [">"] = new Operator(">", 3, Associativity.left),
                    ["|"] = new Operator("|", 1, Associativity.left),
                    ["&"] = new Operator("&", 1, Associativity.left),
                    ["*"] = new Operator("*", 4, Associativity.left),
                    ["/"] = new Operator("/", 4, Associativity.left),
                    ["+"] = new Operator("+", 3, Associativity.left),
                    ["-"] = new Operator("-", 3, Associativity.left),
                    ["^"] = new Operator("^", 5, Associativity.right),
                    ["="] = new Operator("=", 2, Associativity.left)
                };
        }

        /// <summary>
        /// Represents an operator in the input infix string.
        /// </summary>
        private class Operator
        {
            /// <summary>
            /// String representation of the operator.
            /// </summary>
            public string symbol;
            /// <summary>
            /// Priority of the operator.
            /// </summary>
            public int priority;
            /// <summary>
            /// Associativity of the operator.
            /// </summary>
            public Associativity associativity;

            /// <summary>
            /// Constructs an operator.
            /// </summary>
            /// <param name="symbol">String representation of the operator.</param>
            /// <param name="priority">Priority of the operator.</param>
            /// <param name="associativity">Associativity of the operator.</param>
            public Operator(string symbol, int priority, Associativity associativity)
            {
                this.symbol = symbol;
                this.priority = priority;
                this.associativity = associativity;
            }
        }

        /////////////////////////////////////////// Constructor ////////////////////////////////////////////////

        /// <summary>
        /// Constructs an instance of the main interface.
        /// </summary>
        public Form1()
        {
            InitializeComponent();
        }

        ////////////////////////////////////////// Method Declaration ///////////////////////////////////////////

        /// <summary>
        /// Converts an infix notation string to a postfix notation string.
        /// </summary>
        /// <param name="infix">The infix notation string to convert.</param>
        /// <returns>The postfix notation equavalent string.</returns>
        public string ConvertInToPost(string infix)
        {
            // Output string.
            string postfix = "";

            // Break down the infix string to tokens.
            List<string> tokens          = Tokenise(infix);
            // Rearrange and modify the tokens to postfix representation.
            List<string> postfixSequence = ShuntingYard(tokens);
            // Convert the postfix list to a string for output.
            postfix = ToString(postfixSequence);

            return postfix;
        }

        /// <summary>
        /// Converts a postfix notation string to an infix notation string.
        /// </summary>
        /// <param name="postfix">The postfix notation string to convert.</param>
        /// <returns>The infix notation equavalent string.</returns>
        public string ConvertPostToIn(string postfix)
        {
            List<string> tokens = Tokenise(postfix);
            return PostfixToInfix(tokens);
        }

        /// <summary>
        /// Converts the postfix sequence list to a string.
        /// </summary>
        /// <param name="postfixSequence">The postfix sequence to convert.</param>
        /// <returns>A string that represents the passed in postfix sequence.</returns>
        public string ToString(List<string> postfixSequence)
        {
            // If the postfix sequence is not well formed, output "ERROR".
            if (postfixSequence == null)
            {
                return "ERROR";
            }
            else
            {
                // The output string.
                string output = "";

                // Add a space in between all strings in the postfix sequence when building the output string.
                foreach (string str in postfixSequence)
                {
                    // Append the current string to the output.
                    output += str + " ";
                }
                // Get rid of the trailing space char.
                output = output.TrimEnd(' ');
                return output;
            }
        }

        /// <summary>
        /// Uses shunting yard algorithm to convert an infix sequence to a postfix sequence.
        /// </summary>
        /// <param name="tokens">The infix sequence represented in a string token list.</param>
        /// <returns>A list of strings representing the converted postfix sequence.</returns>
        public List<string> ShuntingYard(List<string> tokens)
        {
            // Output list.
            List<string> postfixSequence = new List<string>();
            // Store all symbols on the stack.
            Stack<string> symbolStack = new Stack<string>();

            // Parse tokens sequencially.
            foreach (string token in tokens)
            {
                // If the current token is a non-symbol token, directly add it to the list.
                if (char.IsLetterOrDigit(token[0]))
                {
                    postfixSequence.Add(token);
                }

                // If the current token is an operator symbol:
                else if (Constant.operatorPriority.ContainsKey(token))
                {
                    string operator1 = token;
                    // Keep adding the top operator to the output sequence if operator1's priority
                    // is less than or equal to its priority (left associative); if operator1's 
                    // priority is less than its priority (right associative).
                    while (symbolStack.Count != 0 &&
                        Constant.operatorPriority.ContainsKey(symbolStack.Peek()) &&
                        ((Constant.operatorPriority[operator1].associativity == Associativity.left &&
                        Constant.operatorPriority[operator1].priority <=
                        Constant.operatorPriority[symbolStack.Peek()].priority) ||
                        (Constant.operatorPriority[operator1].associativity == Associativity.right &&
                        Constant.operatorPriority[operator1].priority <
                        Constant.operatorPriority[symbolStack.Peek()].priority)))
                    {
                        postfixSequence.Add(symbolStack.Pop());
                    }
                    // After popping out the operators, push operator1 onto the stack.
                    symbolStack.Push(operator1);
                }

                // If the current token is a left round bracket, push it in.
                else if (token == "(")
                {
                    symbolStack.Push(token);
                }

                // If the current token is a right round bracket:
                else if (token == ")")
                {
                    // Keep popping and adding the top operator to the output sequence until 
                    // the top element is a left round bracket.
                    while (symbolStack.Count != 0 && symbolStack.Peek() != "(")
                    {
                        postfixSequence.Add(symbolStack.Pop());
                    }

                    // If we already reached the bottom of the stack without finding 
                    // a left round bracket, notice the user about the error.
                    if (!(symbolStack.Count != 0 && symbolStack.Peek() == "("))
                    {
                        MessageBox.Show("Could not pair all brackets in the input.",
                            "Syntax Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return null;
                    }

                    else
                    {
                        // Pop the left round bracket out without adding it to the sequence.
                        symbolStack.Pop();
                    }
                }

                // Unrecognised token:
                else
                {
                    MessageBox.Show("Unsupported symbol \'" + token + "\' in the input.",
                            "Unrecognised Symbol", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
            }

            // After going through the sequence, pop all elements on the stack out to the output.
            while (symbolStack.Count != 0)
            {
                // If there are still brackets on the stack, it means a mismatch.
                if (symbolStack.Peek() == "(" || symbolStack.Peek() == ")")
                {
                    MessageBox.Show("Could not pair all brackets in the input.",
                                    "Syntax Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
                // Otherwise, just pop and add the symbols to the sequence.
                else
                {
                    postfixSequence.Add(symbolStack.Pop());
                }
            }

            return postfixSequence;
        }

        /// <summary>
        /// Converts a postfix sequence to its infix equavalent sequence.
        /// </summary>
        /// <param name="tokens">The postfix sequence represented in a string token list.</param>
        /// <returns>A string representing the converted infix sequence.</returns>
        public string PostfixToInfix(List<string> tokens)
        {
            // Stack to temporarily store the parsed result.
            Stack<string> symbolStack = new Stack<string>();

            // If empty, return empty.
            if (tokens.Count == 0)
            {
                return "";
            }

            // If only one, return only one.
            else if (tokens.Count == 1)
            {
                return tokens[0];
            }

            // More than one token:
            for (int i = 0; i < tokens.Count; i++)
            {
                // Get the current token.
                string token = tokens[i];

                // If it is an operand, push it onto the stack.
                if (!Constant.operatorPriority.ContainsKey(token))
                {
                    symbolStack.Push(token);
                }
                // The token is an operator:
                else
                {
                    // If there are fewer than 2 values on the stack, error!
                    if (symbolStack.Count < 2)
                    {
                        // Error message:
                        MessageBox.Show("Not enough operands present to evaluate the Postfix string.",
                            "Missing Operand", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return "ERROR";
                    }
                    // Else form an operation string.
                    else
                    {
                        // Reverse the sequence, because last in first out.
                        string operand2 = symbolStack.Pop();
                        string operand1 = symbolStack.Pop();

                        if (Constant.operatorPriority.ContainsKey(token))
                        {
                            string operation = "";
                            // Wrap the operation with round brackets.
                            operation = "(" + operand1 + " " + token + " " + operand2 + ")";
                            symbolStack.Push(operation);
                        }

                        else
                        {
                            MessageBox.Show("Invalid operator.", "Error", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            return "ERROR";
                        }
                    }
                    
                    // If there is only one symbol on the stack and we have reached the end of the 
                    // string list, output the result.
                    if (symbolStack.Count == 1 && i == tokens.Count - 1)
                    {
                        // Remove the outter round brackets.
                        string result = symbolStack.Pop().Remove(0, 1);
                        result = result.Remove(result.Length - 1, 1);
                        return result;
                    }
                }
            }
            // If the function was not able to return the value before this point, it means that there is
            // something wrong with the input postfix string.
            return "ERROR";
        }

        /// <summary>
        /// Tokenises a string. Splits the string when there is a symbol/non-letter/digit character.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private List<string> Tokenise(string str)
        {
            // Output list.
            List<string> tokens = new List<string>();

            // Hold the current token string here.
            string currentToken = "";

            // Go through every character in the input string to parse out tokens.
            foreach (char c in str)
            {
                // If the current character is a letter or digit, queue it to build up the token.
                // Added '.' to account for float values.
                if (char.IsLetterOrDigit(c) || c == '.')
                {
                    currentToken += c;
                }
                // If the current character is not a letter, digit, or space, it makes up a single token.
                // In this case, we just add it to the output token list.
                else
                {
                    // If there is an existing stored string before running into this symbol, it means that
                    // we need to store both the existing string and the current character in the token list.
                    if (currentToken != "")
                    {
                        tokens.Add(currentToken);
                        // Clear the token string after storing the current token.
                        currentToken = "";
                    }
                    if (c != ' ')
                    {
                        // Now deal with the symbol.
                        currentToken = c.ToString();
                        tokens.Add(currentToken);
                    }
                    // After adding a token to the list, clear the currentToken string.
                    currentToken = "";
                }
            }

            // If there is a leftover string, add that to the list too.
            if (currentToken != "")
            {
                tokens.Add(currentToken);
            }

            return tokens;
        }

        ///////////////////////////////////////////// Control ////////////////////////////////////////////////////

        /// <summary>
        /// Triggered when the InToPost button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InToPostButton_Click(object sender, EventArgs e)
        {
            // Convert!
            PostfixTextBox.Text = ConvertInToPost(InfixTextBox.Text);
        }

        /// <summary>
        /// Triggered when the PostToIn button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PostToInButton_Click(object sender, EventArgs e)
        {
            // Convert!
            InfixTextBox.Text = ConvertPostToIn(PostfixTextBox.Text);
        }

        ///////////////////////////////////////// Enumeration //////////////////////////////////////////////////

        /// <summary>
        /// Associativity of the operators.
        /// </summary>
        private enum Associativity
        {
            left,
            right
        }
    }
}
