// Выполнили студенты группы 221701 БГУИР:
// - Карпук Максим Витальевич (oden73)
// - Телица Илья Денисович
// Вариант 4
//
// Класс для хранения бинарного дерева, которое используется в классе LogicalExpression
// 22.05.2024
//
// Источники:
// - Проектирование программного обеспечения интеллектуальных систем (3 семестр)
//



namespace LOIS1
{
	public class BinaryTree
	{
		public String key { get; set; }
		public List<int> subexpression_indexes { get; set; }
		public BinaryTree? left_child { get; set; }
		public BinaryTree? right_child { get; set; }

		public BinaryTree()
        {
			this.key = "";
			this.subexpression_indexes = new List<int>();
			this.left_child = null;
			this.right_child = null;
		}

		public BinaryTree(String root_object, List<int> indexes)
		{
			this.key = root_object;
			this.subexpression_indexes = indexes;
			this.left_child = null;
			this.right_child = null;
		}

		public void insert_left(String new_key, List<int> new_indexes)
        {
			this.left_child = new BinaryTree(new_key, new_indexes);
        }

		public void insert_right(String new_key, List<int> new_indexes)
        {
			this.right_child = new BinaryTree(new_key, new_indexes);
        }
	}
}
