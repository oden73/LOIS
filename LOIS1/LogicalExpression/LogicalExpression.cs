// Выполнили студенты группы 221701 БГУИР:
// - Карпук Максим Витальевич (oden73)
// - Телица Илья Денисович
// Вариант 4
// 
// Класс для хранения логического выражения
// 23.05.2024
//
// Источники:
// - Проектирование программного обеспечения интеллектуальных систем (3 семестр)
//


namespace LOIS1
{
    public class LogicalExpression
    {
        private string expression;
        private BinaryTree syntax_tree;
        private Dictionary<string, int> indexes_dict;
        private Dictionary<int, string> values_dict;

        public LogicalExpression(string expression)
        {
            this.expression = expression;

            this.initial_validation();

            this.syntax_tree = this.form_syntax_tree();
            this.indexes_dict = new Dictionary<string, int>();
            this.values_dict = new Dictionary<int, string>();
            this.form_indexes_and_values_dict();
        }

        public void initial_validation()
        {
            if (string.IsNullOrEmpty(this.expression))
            {
                throw new ArgumentNullException("Введеное выражение не является корректным");
            }

            if (this.expression.Length == 1)
            {
                if(this.expression[0] <= 'Z' && this.expression[0] >= 'A')
                {
                    return;
                }
                else
                {
                    throw new ArgumentNullException("Введеное выражение не является корректным");
                }
            }
            else
            {
                if (this.expression[0] != '(' && this.expression[0] != '!')
                {
                    throw new ArgumentNullException("Введеное выражение не является корректным");
                }
                else
                {
                    List<int> marked_brackets = new List<int>();
                    this.expression_validation(1, ref marked_brackets);
                }
            }
        }

        public void expression_validation(int start_index, ref List<int> marked_brackets)
        {
            bool walked = false;
            for (int i = start_index; i < this.expression.Length; i++)
            {
                if (this.expression[i] == '(' && !walked)
                {
                    this.expression_validation(i + 1, ref marked_brackets);
                    walked = true;
                }
                else if (this.expression[i] == ')' && !marked_brackets.Exists(element => element == i) && i - start_index + Convert.ToInt32(start_index == 1) >= 2)
                {
                    marked_brackets.Add(i);
                    return;
                }
            }

            throw new ArgumentException("Введеное выражение не является корректным");
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
                    BinaryTree top_element = parent_stack.First();
                    if (top_element.key == "")
                    {
                        BinaryTree parent = parent_stack.Pop();
                        parent.key = "!";
                        parent_stack.Push(parent);
                    }
                    else
                    {
                        current_subtree.key = "!";
                        current_subtree.insert_left("", new List<int>());
                        current_subtree.subexpression_indexes.Add(i);
                        parent_stack.Push(current_subtree);
                        if (current_subtree.left_child != null)
                        {
                            current_subtree = current_subtree.left_child;
                        }
                    }
                }
                else if (this.is_letter(this.expression[i]) || this.is_constant(this.expression[i]))
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
                    throw new ArgumentException("Введеное выражение не является корректным");
                }
            }
            return syntax_tree;
        }

        public void form_indexes_and_values_dict()
        {
            List<string> chars = new List<string>();
            for (int i = 0; i < this.expression.Length; i++)
            {
                if ((this.expression[i] >= 'A' && this.expression[i] <= 'Z' || this.is_constant(this.expression[i])) &&
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
            if (syntax_subtree.key == "" && (syntax_subtree.left_child != null && !this.is_letter(syntax_subtree.left_child.key[0])))
            {
                throw new ArgumentException("Введенное выражение не является корректным");
            }
            BinaryTree left_child = syntax_subtree.left_child, right_child = syntax_subtree.right_child;
            if (left_child != null && right_child != null)
            {
                switch (syntax_subtree.key)
                {
                    case "/":
                        bool value = this.disjunction(this.evaluate(left_child, values_list), this.evaluate(right_child, values_list));
                        return value;
                    case "\\":
                        value = this.conjunction(this.evaluate(left_child, values_list), this.evaluate(right_child, values_list));
                        return value;
                    case ">":
                        value = this.implication(this.evaluate(left_child, values_list), this.evaluate(right_child, values_list));
                        return value;
                    case "~":
                        value = this.equivalence(this.evaluate(left_child, values_list), this.evaluate(right_child, values_list));
                        return value;
                    default:
                        throw new ArgumentException("Введеное выражение не является корректным");
                }
            }
            else if (left_child != null && right_child == null)
            {
                bool value = this.negation(this.evaluate(left_child, values_list));
                return value;
            }
            else
            {
                bool value = false;
                if (this.is_constant(syntax_subtree.key[0]))
                {
                    value = syntax_subtree.key == "1";
                    return value;
                }
                value = values_list[this.indexes_dict[syntax_subtree.key]];
                return value;
            }
        }

        public bool is_neutral()
        {
            List<bool> final_values = new List<bool>();
            int bits = this.amount_of_operands();

            bool value = false;

            List<bool> number = Enumerable.Repeat(false, bits).ToList();
            bool first_value = this.evaluate(this.syntax_tree, number);

            for (int i = 0; i < Math.Pow(2, bits); i++)
            {
                if (this.evaluate(this.syntax_tree, number) != first_value)
                {
                    value = true;
                    return value;
                }

                number = this.increment(number);
            }

            return value;
        }

        public int amount_of_operands()
        {
            List<string> operands = new List<string>();

            for (int i = 0; i < this.expression.Length; i++)
            {
                if ((this.expression[i] >= 'A' && this.expression[i] <= 'Z' || this.is_constant(this.expression[i])) &&
                    !operands.Exists(element => element == "" + this.expression[i]))
                {
                    operands.Add("" + this.expression[i]);
                }
            }

            int value = operands.Count;
            return value;
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
            bool value = symbol >= 'A' && symbol <= 'Z';
            return value;
        }

        public bool is_constant(char symbol)
        {
            bool value = symbol == '0' || symbol == '1';
            return value;
        }

        public bool is_symbol(int index)
        {
            bool value = false;
            if (this.expression == null)
            {
                return value;
            }
            value = (this.expression[index] == '\\' && this.expression[index - 1] == '/') ||
                (this.expression[index] == '/' && this.expression[index - 1] == '\\') || this.expression[index] == '~'
                || (this.expression[index] == '>' && this.expression[index - 1] == '-');
            return value;
        }

        public bool conjunction(bool first, bool second)
        {
            bool value = first && second;
            return value;
        }

        public bool disjunction(bool first, bool second)
        {
            bool value = first || second;
            return value;
        }

        public bool implication(bool first, bool second)
        {
            bool value = !first || second;
            return value;
        }

        public bool equivalence(bool first, bool second)
        {
            bool value = first == second;
            return value;
        }

        public bool negation(bool first)
        {
            bool value = !first;
            return value;
        }
    }

}
