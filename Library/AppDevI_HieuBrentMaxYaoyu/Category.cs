using System;

namespace Budget
{
    /// <summary>
    /// Represents a <see cref="Category"/> item.
    /// </summary>
    public class Category
    {
        private int _id;
        private string _description;
        private CategoryType _type;

        /// <summary>
        /// Describes the type of the <see cref="Category"/>.
        /// </summary>
        public enum CategoryType
        {
            Income = 1,
            Expense,
            Credit,
            Savings
        };

        /// <summary>
        /// Initializes a new instance of <see cref="Category"/> class with the given information.
        /// </summary>
        /// <param name="id">The id of the <see cref="Category"/>.</param>
        /// <param name="description">The description of the <see cref="Category"/>.</param>
        /// <param name="type">The type of the <see cref="Category"/>.</param>
        public Category(int id, string description, CategoryType type = CategoryType.Expense)
        {
            Id = id;
            Description = description;
            Type = type;
        }

        /// <summary>
        /// Returns the id of the <see cref="Category"/>.
        /// </summary>
        public int Id
        {
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
        /// The description of the <see cref="Category"/>.
        /// </summary>
        public string Description
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

        /// <summary>
        /// The type of the <see cref="Category"/>.
        /// </summary>
        public CategoryType Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        /// <summary>
        /// Returns the <see cref="string"/> representation of the <see cref="Category"/>.
        /// </summary>
        /// <returns>The <see cref="string"/> representation of the <see cref="Category"/>.</returns>
        public override string ToString()
        {
            return Type + ": " + Description;
        }
    }
}