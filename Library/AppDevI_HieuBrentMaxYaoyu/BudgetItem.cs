using System;
using System.Collections.Generic;

namespace Budget
{
    /// <summary>
    /// Represents a <see cref="BudgetItem"/> with the information about the <see cref="Category"/> and the <see cref="Expense"/>.
    /// </summary>
    public class BudgetItem
    {
        /// <summary>
        /// Initializes a new instance of <see cref="BudgetItem"/> class.
        /// </summary>
        public BudgetItem() { }

        /// <summary>
        /// The <see cref="Category"/> id.
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// The <see cref="Expense"/> id.
        /// </summary>
        public int ExpenseId { get; set; }

        /// <summary>
        /// The date of the <see cref="Expense"/>.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// The description of the <see cref="Category"/>.
        /// </summary>
        public string CategoryDescription { get; set; }

        /// <summary>
        /// The description of the <see cref="Expense"/>.
        /// </summary>
        public string ExpenseDescription { get; set; }

        /// <summary>
        /// The amount of the <see cref="Expense"/>.
        /// </summary>
        public double Amount { get; set; }

        /// <summary>
        /// The current balance upon this <see cref="Expense"/> (also after all the previous ones).
        /// </summary>
        public double Balance { get; set; }
    }

    /// <summary>
    /// Represents a list of <see cref="BudgetItem"/> in the same current <see cref="Month"/>.
    /// </summary>
    public class BudgetItemsByMonth
    {
        /// <summary>
        /// The month of the <see cref="BudgetItem"/>.
        /// </summary>
        public string Month { get; set; }

        /// <summary>
        /// The list of all <see cref="BudgetItem"/> in the same current <see cref="Month"/>.
        /// </summary>
        public List<BudgetItem> Details { get; set; }

        /// <summary>
        /// The total of all <see cref="Expense.Amount"/> in the current <see cref="Month"/>.
        /// </summary>
        public double Total { get; set; }
    }

    /// <summary>
    /// Represents a list of <see cref="BudgetItem"/> with the same <see cref="Category"/>.
    /// </summary>
    public class BudgetItemsByCategory
    {
        /// <summary>
        /// The category description of the <see cref="BudgetItem"/>.
        /// </summary>
        public string CategoryDescription { get; set; }

        /// <summary>
        /// The list of all <see cref="BudgetItem"/> with the same <see cref="Category"/>.
        /// </summary>
        public List<BudgetItem> Details { get; set; }

        /// <summary>
        /// The total of all <see cref="Expense.Amount"/> with the same <see cref="Category"/>.
        /// </summary>
        public double Total { get; set; }
    }
}
