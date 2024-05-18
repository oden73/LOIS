using System;

namespace LOIS1
{
    public class LogicalExpression
    {
        private String expression;
        private BinaryTree syntax_tree;
        Dictionary<String, int> indexes_dict;
        Dictionary<int, String> values_dict;

        public LogicalExpression(String expression)
        {
            this.expression = expression;
            this.syntax_tree = this.form_syntax_tree();
            this.indexes_dict = new Dictionary<String, int>();
            this.values_dict = new Dictionary<int, String>();
            this.form_indexes_and_values_dict();
        }

        public BinaryTree form_syntax_tree()
        {
            Stack<BinaryTree> parent_stack = new Stack<BinaryTree>();
            BinaryTree syntax_tree = new BinaryTree();
            parent_stack.Push(syntax_tree);
            BinaryTree current_subtree = syntax_tree;
            for (int i = 0; i < this.expression.Length; i++)
            {
                if (this.expression[i] == '(')
                {
                    current_subtree.insert_left("", new List<int>());
                    current_subtree.subexpression_indexes.Add(i);
                    parent_stack.Push(current_subtree);
                    if (current_subtree.left_child != null)
                    {
                        current_subtree = current_subtree.left_child;
                    }
                }
                else if (this.expression[i] == '!')
                {
                    BinaryTree parent = parent_stack.Pop();
                    parent.key = "¬";
                    parent_stack.Push(parent);
                }
                else if (this.is_letter(this.expression[i]))
                {
                    current_subtree.key = "" + this.expression[i];
                    BinaryTree parent = parent_stack.Pop();
                    current_subtree = parent;
                }
                else if (this.expression[i] == '-' || (this.expression[i] == '/' && this.expression[i - 1] != '\\')
                    || (this.expression[i] == '\\' && this.expression[i - 1] != '/'))
                {
                    continue;
                }
                else if (this.is_symbol(i))
                {
                    current_subtree.key = "" + this.expression[i];
                    current_subtree.insert_right("", new List<int>());
                    parent_stack.Push(current_subtree);
                    if (current_subtree.right_child != null)
                    {
                        current_subtree = current_subtree.right_child;
                    }
                }
                else if (this.expression[i] == ')')
                {
                    current_subtree.subexpression_indexes.Add(i);
                    current_subtree = parent_stack.Pop();
                }
                else
                {
                    throw new ArgumentException("Invalid expression");
                }
            }
            return syntax_tree;
        }


        public void form_indexes_and_values_dict()
        {
            List<String> chars = new List<String>();
            for (int i = 0; i < this.expression.Length; i++)
            {
                if (this.expression[i] >= 'A' && this.expression[i] <= 'Z' && 
                    !chars.Exists(element => element == "" + this.expression[i]))
                {
                    chars.Add("" + this.expression[i]);
                }
            }
            chars.Sort();
            for (int i = 0; i < chars.Count; i++)
            {
                this.indexes_dict[chars[i]] = i;
                values_dict[i] = chars[i]; 
            }
        }

        public bool evaluate(BinaryTree syntax_subtree, List<bool> values_list)
        {
            BinaryTree? left_child = syntax_subtree.left_child, right_child = syntax_subtree.right_child;
            if (left_child != null && right_child != null)
            {
                switch (syntax_subtree.key)
                {
                    case "/":
                        return this.disjunction(this.evaluate(left_child, values_list), this.evaluate(right_child, values_list));
                    case "\\":
                        return this.conjunction(this.evaluate(left_child, values_list), this.evaluate(right_child, values_list));
                    case ">":
                        return this.implication(this.evaluate(left_child, values_list), this.evaluate(right_child, values_list));
                    case "~":
                        return this.equivalence(this.evaluate(left_child, values_list), this.evaluate(right_child, values_list));
                    default:
                        throw new ArgumentException("Invalid expression");
                }
            }
            else if (left_child != null && right_child == null)
            {
                return this.negation(this.evaluate(left_child, values_list));
            }
            else
            {
                return values_list[this.indexes_dict[syntax_subtree.key]];
            }
        }

        public List<String> truth_table_print_strings()
        {
            String print_string = "";
            List<String> output_lines = new List<String>();
            List<LogicalExpression> subtrees = new List<LogicalExpression>();
            BinaryTree tree = this.syntax_tree;

            this.get_subtrees(tree, ref subtrees);
            subtrees.Reverse();
            
            List<String> operands = new List<String>();
            int bits = this.operands_and_their_amount(ref operands);
            
            List<bool> number = Enumerable.Repeat(false, bits).ToList();
            
            for (int i = 0; i < operands.Count; i++)
            {
                print_string += $"I {operands[i]} ";
            }
            
            for (int i = 0; i < subtrees.Count; i++)
            {
                print_string += $"I {subtrees[i].expression} ";
            }
            
            output_lines.Add(print_string);
            
            for (int i = 0; i < Math.Pow(2, bits); i++)
            {
                print_string = "";
                
                for (int j = 0; j < number.Count; j++)
                {
                    print_string += $"I {Convert.ToString(Convert.ToInt32(number[j]))} ";
                }
                
                for (int j = 0; j < subtrees.Count; j++)
                {
                    print_string += "I ";
                    
                    for (int k = 0; k < subtrees[j].expression.Length / 2 - 
                        Convert.ToInt32(subtrees[j].expression.Length % 2 == 0); k++)
                    {
                        print_string += " ";
                    }

                    print_string += $"{Convert.ToString(Convert.ToInt32(this.evaluate(subtrees[j].syntax_tree, number)))} ";
                    
                    for (int k = 0; k < subtrees[j].expression.Length / 2; k++)
                    {
                        print_string += " ";
                    }
                }

                output_lines.Add(print_string);
                number = this.increment(number);
            }

            return output_lines;
        }

        public void get_subtrees(BinaryTree current_subtree, ref List<LogicalExpression> subtrees)
        {
            if (current_subtree.subexpression_indexes.Count > 0)
            {
                String subexpression = this.expression.Substring(current_subtree.subexpression_indexes[0],
                    current_subtree.subexpression_indexes[1] - current_subtree.subexpression_indexes[0] + 1);
                subtrees.Add(new LogicalExpression(subexpression));
            }
            if (current_subtree.left_child != null)
            {
                get_subtrees(current_subtree.left_child, ref subtrees);
            }
            if (current_subtree.right_child != null)
            {
                get_subtrees(current_subtree.right_child, ref subtrees);
            }
        }

        public bool is_neutral()
        {
            List<bool> final_values = new List<bool>();
            List<String> useless_operands = new List<String>();
            int bits = this.operands_and_their_amount(ref useless_operands);

            List<bool> number = Enumerable.Repeat(false, bits).ToList();
            bool first_value = this.evaluate(this.syntax_tree, number);

            for(int i = 0; i < Math.Pow(2, bits); i++)
            {
                if (this.evaluate(this.syntax_tree, number) != first_value)
                {
                    return true;
                }

                number = this.increment(number);
            }

            return false;
        }

        public int operands_and_their_amount(ref List<String> operands)
        {
            for (int i = 0; i < this.expression.Length; i++)
            {
                if (this.expression[i] >= 'A' && this.expression[i] <= 'Z' &&
                    !operands.Exists(element => element == "" + this.expression[i]))
                {
                    operands.Add("" + this.expression[i]);
                }
            }
            return operands.Count;
        }

        public List<bool> increment(List<bool> number)
        {
            int position = number.Count - 1;
            while (number[position])
            {
                number[position] = false;
                position--;
                if (position < 0)
                {
                    return number;
                }
            }
            number[position] = true;
            return number;
        }

        public bool is_letter(char symbol)
        {
            return symbol != '/' && symbol != '\\' && symbol != '>' && symbol != '~' && symbol != ')' && symbol != '-';
        }

        public bool is_symbol(int index)
        {
            if(this.expression == null)
            {
                return false;
            }
            return (this.expression[index] == '\\' && this.expression[index - 1] == '/') || 
                (this.expression[index] == '/' && this.expression[index - 1] == '\\') || this.expression[index] == '~'
                || (this.expression[index] == '>' && this.expression[index - 1] == '-');
        }

        public bool conjunction(bool first, bool second)
        {
            return first && second;
        }

        public bool disjunction(bool first, bool second)
        {
            return first || second;
        }

        public bool implication(bool first, bool second)
        {
            return !first || second;
        }

        public bool equivalence(bool first, bool second)
        {
            return first == second;
        }

        public bool negation(bool first)
        {
            return !first;
        }

    }
}