using ExpenseTrackerApp;
using ExpenseTrackerApp.Frontend;
using Moq;
using NUnit.Framework;
using System;

namespace ExpenseTrackerTest.Tests
{
    [TestFixture]
    public class RecordExpensesTabTests
    {
        private RecordExpensesTab _recordExpensesTab;
        private Mock<IMainWindow> _mockMainWindow; // Assuming an interface for MainWindow

        [SetUp]
        public void Setup()
        {
            _mockMainWindow = new Mock<IMainWindow>();
            //_recordExpensesTab = new RecordExpensesTab(_mockMainWindow.Object);
            // Initialize other necessary UI elements and dependencies
        }

        [Test]
        public void AddExpenseButton_Click_ValidInput_AddsExpense()
        {
            // Arrange
            _recordExpensesTab.DescriptionTextBox.Text = "Groceries";
            _recordExpensesTab.AmountTextBox.Text = "$50";
            _recordExpensesTab.DatePicker.SelectedDate = DateTime.Now;
            // Set TypeComboBox and CategoryComboBox to valid values

            // Act
            //_recordExpensesTab.AddExpenseButton_Click(null, null); // Simulate button click

            // Assert
            _mockMainWindow.Verify(mw => mw.AddExpense("Expenses", It.IsAny<Expense>()), Times.Once);
        }

        [Test]
        public void AddExpenseButton_Click_InvalidInput_ShowsErrorMessage()
        {
            // Arrange
            _recordExpensesTab.DescriptionTextBox.Text = ""; // Invalid input
            _recordExpensesTab.AmountTextBox.Text = "invalid";
            _recordExpensesTab.DatePicker.SelectedDate = null;

            // Act
            //_recordExpensesTab.AddExpenseButton_Click(null, null); // Simulate button click

            // Assert
            // Assert that an error message box is shown. This might require a service for message boxes to be mockable.
        }
    }
}
