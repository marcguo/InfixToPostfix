using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace InfixToPostfix
{
    /// <summary>
    /// Main interface of the program.
    /// </summary>
    public partial class Form1 : Form
    {
        /// <summary>
        /// Stores the constant values.
        /// </summary>
        private static class Constant
        {
            /// <summary>
            /// Contains priorities for accepted operators.
            /// </summary>
            public static readonly Dictionary<string, int> operatorPriority = new Dictionary<string, int>()
            {
                ["<"] = 1,
                [">"] = 1,
                ["|"] = 1,
                ["&"] = 1,
                ["*"] = 2,
                ["/"] = 2,
                ["+"] = 1,
                ["-"] = 1,
                ["^"] = 3
            };
        }

        /// <summary>
        /// Constructs an instance of the main interface.
        /// </summary>
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Converts an infix notation string to a postfix notation string.
        /// </summary>
        /// <param name="infix">The infix notation string to convert.</param>
        /// <returns>The postfix notation equavalent string.</returns>
        private string Convert(string infix)
        {
            // Output string.
            string postfix = "";

            List<string> tokens          = Tokenise(infix);
            List<string> postfixSequence = ShuntingYard(tokens);
            postfix = ToString(postfixSequence);

            return postfix;
        }

        /// <summary>
        /// Converts the postfix sequence list to a string.
        /// </summary>
        /// <param name="postfixSequence">The postfix sequence to convert.</param>
        /// <returns>A string that represents the passed in postfix sequence.</returns>
        private string ToString(List<string> postfixSequence)
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
        private List<string> ShuntingYard(List<string> tokens)
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
                    // is less than or equal to its priority.
                    while (symbolStack.Count != 0 &&
                        Constant.operatorPriority.ContainsKey(symbolStack.Peek()) &&
                        Constant.operatorPriority[operator1] <=
                        Constant.operatorPriority[symbolStack.Peek()])
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
                else
                {
                    postfixSequence.Add(symbolStack.Pop());
                }
            }

            return postfixSequence;
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
                if (char.IsLetterOrDigit(c))
                {
                    currentToken += c;
                }
                // If the current character is not a letter, digit, or space, it makes up a single token.
                // In this case, we just add it to the output token list.
                else if (c != ' ')
                {
                    // If there is an existing stored string before running into this symbol, it means that
                    // we need to store both the existing string and the current character in the token list.
                    if (currentToken != "")
                    {
                        tokens.Add(currentToken);
                        // Clear the token string after storing the current token.
                        currentToken = "";
                    }
                    // Now deal with the symbol.
                    currentToken = c.ToString();
                    tokens.Add(currentToken);
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

        /// <summary>
        /// Triggered when the Convert button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConvertButton_Click(object sender, EventArgs e)
        {
            // Convert!
            PostfixTextBox.Text = Convert(InfixTextBox.Text);
        }
    }
}
