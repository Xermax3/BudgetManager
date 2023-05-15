using System;

namespace Budget
{
    /// <summary>
    /// Represents an <see cref="Expense"/> item.
    /// </summary>
    public class Expense
    {
        private int _id;
        private DateTime _date;
        private int _categoryId;
        private double _amount;
        private string _description;

        /// <summary>
        /// Initializes an instance of <see cref="Expense"/> class with the given information.
        /// </summary>
        /// <param name="id">The id of the expense</param>
        /// <param name="date">The date of the expense</param>
        /// <param name="categoryId">The category of the expense</param>
        /// <param name="amount">The amount of the expense</param>
        /// <param name="description">The description of the expense</param>
        public Expense(int id, DateTime date, int categoryId, Double amount, String description)
        {
            Id = id;
            Date = date;
            CategoryId = categoryId;
            Amount = amount;
            Description = description;
        }

        /// <summary>
        /// Initializes an instance of <see cref="Expense"/> class with the information of another <see cref="Expense"/> instance.
        /// </summary>
        /// <param name="obj">The existing <see cref="Expense"/> instance.</param>
        public Expense(Expense obj)
        {
            Id = obj.Id;
            Date = obj.Date;
            CategoryId = obj.CategoryId;
            Amount = obj.Amount;
            Description = obj.Description;
        }

        /// <summary>
        /// Returns the id of the <see cref="Expense"/>.
        /// </summary>
        public int Id {
            get
            {
                return _id;
            }
            private set
            {
                _id = value;
            }
        }

        /// <summary>
        /// The date of the <see cref="Expense"/>.
        /// </summary>
        public DateTime Date {
            get
            {
                return _date;
            }
            set
            {
                _date = value;
            }
        }

        /// <summary>
        /// The <see cref="Category"/> id of the <see cref="Expense"/>.
        /// </summary>
        public int CategoryId
        {
            get
            {
                return _categoryId;
            }
            set
            {
                _categoryId = value;
            }
        }

        /// <summary>
        /// The amount of the <see cref="Expense"/>.
        /// </summary>
        public double Amount
        {
            get
            {
                return _amount;
            }
            set
            {
                _amount = value;
            }
        }

        /// <summary>
        /// The description of the <see cref="Expense"/>.
        /// </summary>
        public String Description 
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
            }
        }
    }
}
