// ��������� �������� ������ 221701 �����:
// - ������ ������ ���������� (oden73)
// - ������ ���� ���������
// ������� 4
// 
// ����� ��� �������� unit-test��, ������� ��������� � ������ LogicalExpression
// 22.05.2024
//
// ���������:
// - �������������� ������������ ����������� ���������������� ������ (3 �������)
// - ���������� xUnit
//


using LOIS1;

namespace TestLogicalExpression
{
    [TestClass]
    public class TestLogicalExpression
    {
        [TestMethod]
        public void TestLogicalExpression1()
        {
            String expression = "(A\\/(B\\/(C\\/(D\\/(E\\/(F\\/(G\\/(H\\/(I\\/(J\\/(K\\/(L\\/(M\\/(N\\/(O\\/(P\\/(Q\\/(R\\/(S\\/(T\\/(U\\/(V\\/(W\\/(X\\/(Y\\/Z)))))))))))))))))))))))))";
            LogicalExpression logical_expression = new LogicalExpression(expression);

            Assert.IsTrue(logical_expression.is_neutral());
        }

        [TestMethod]
        public void TestLogicalExpression2()
        {
            String expression = "(A->(B~(!(C\\/A))))";
            LogicalExpression logical_expression = new LogicalExpression(expression);

            Assert.IsTrue(logical_expression.is_neutral());
        }

        [TestMethod]
        public void TestLogicalExpression3()
        {
            String expression = "((A/\\(B/\\C))\\/(((!A)/\\((!B)/\\D))\\/(E/\\(F/\\G))))";
            LogicalExpression logical_expression = new LogicalExpression(expression);

            Assert.IsTrue(logical_expression.is_neutral());
        }

        [TestMethod]
        public void TestLogicalExpression4()
        {
            String expression = "(A\\/(!A))";
            LogicalExpression logical_expression = new LogicalExpression(expression);

            Assert.IsTrue(!logical_expression.is_neutral());
        }

        [TestMethod]
        public void TestLogicalExpression5()
        {
            String expression = "((A->((C~D)/\\(B\\/E)))~(A->((C~D)/\\(B\\/E))))";
            LogicalExpression logical_expression = new LogicalExpression(expression);

            Assert.IsTrue(!logical_expression.is_neutral());
        }
    }
}