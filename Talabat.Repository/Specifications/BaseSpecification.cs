using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Interfaces;

namespace Talabat.Core.Specifications
{
    public class BaseSpecification<T> : ISpecification<T> where T : BaseEntity
    {
        /*
         * In C#, the Expression<Func<T, bool>> represents a lambda expression that is translated into an expression tree. 
         * This is a core part of how LINQ works, especially when building dynamic queries. Let's break it down:
         * 
         * 1. Expression Trees
         *      An Expression is a way to represent code (specifically, lambda expressions) 
         *      as a data structure (an expression tree) rather than just compiled executable code.
         *      This allows the lambda expression to be inspected, modified, or translated at runtime, rather than simply executed.
         *      In your case, Expression<Func<T, bool>> is a lambda that takes an instance of T (the generic type) and returns a bool.
         * 2. Lambda Expressions
         *      A lambda expression is essentially a shorthand for a function. For example:
         *      Ex: 
         *      Func<int, bool> isEven = x => x % 2 == 0;
         *      Here, Func<int, bool> is a function that takes an integer (int) and returns a boolean (bool). 
         *      The lambda x => x % 2 == 0 checks if x is even.
         * 
         * 3. Expression<Func<T, bool>>
         *      Now, when we declare:
         *      Ex:
         *      public Expression<Func<T, bool>> Criteria { get; set; }
         *      It means:
         *      Func<T, bool>: A function that takes an object of type T and returns a bool (for example, checking a condition).
         *      Expression<Func<T, bool>>: Instead of just storing a lambda function, we store an expression tree 
         *      representing the function. This allows it to be compiled into an actual SQL query (or another expression) at runtime.
         *  4. Usage in the Specification Pattern
         *      In the Specification Pattern, Expression<Func<T, bool>> is used to define a condition or filter 
         *      for querying entities from the database. The expression is translated into a SQL WHERE clause 
         *      when using Entity Framework or other LINQ providers.
         * 
         *      Example
         *      Let's say you want to filter products where the Id is equal to 5. You could define this with:
         * 
         *      public class ProductWithIdSpecification : BaseSpecification<Product>
         *      {
         *          public ProductWithIdSpecification(int id): base(p => p.Id == id){}
         *      }    
         *      Here, p => p.Id == id is a lambda expression, which checks whether the Id of the product matches the given id.
         * 
         * Why Use Expression<Func<T, bool>> Instead of Func<T, bool>?
         *      The key difference is that:
         *      Func<T, bool> can only be executed in-memory. It cannot be converted into a SQL query.
         *      Expression<Func<T, bool>> can be parsed by LINQ providers (like Entity Framework) and translated into SQL.
         * 
         */
        #region Why I can't use Func<T, bool>
        /*
         * 1. Func<T, bool>: Directly Compiled Code
         *      A Func<T, bool> is simply a delegate reference to a method (or lambda expression) that can be executed in memory.
         * When you use Func<T, bool>, the logic inside the lambda expression is compiled and executed in memory by the .NET runtime.
         * Therefore:
         *      It can only be executed on in-memory objects (i.e., after data has already been loaded from the database).
         *      It cannot be translated into another format (like SQL), because once it's compiled, 
         *      it's just code that the CPU runs—it’s not accessible as a "queryable structure."
         *
         * 2. Expression<Func<T, bool>>: An Expression Tree
         *      An Expression<Func<T, bool>>, on the other hand, does not immediately execute the code. 
         *      Instead, it builds an expression tree that represents the logic as data rather than code.
         *      For example, if you define:
         *          Expression<Func<int, bool>> isEvenExpr = x => x % 2 == 0;
         *          This does not compile directly into a function to be executed in memory.
         *          Instead, the expression is stored as a data structure that describes the logic: 
         *              “take a number x, apply the modulus operator %, compare the result with 0.” 
         *          The tree is a detailed breakdown of this logic.
         *      Since Expression<Func<T, bool>> is stored as data, frameworks like Entity Framework can parse it and translate it into SQL.
         * 
         * 3. Why Func<T, bool> Cannot Be Translated to SQL
         *      The fundamental reason is that Func<T, bool> is already compiled code:
         *          Once a Func<T, bool> exists, the actual logic is embedded in the compiled .NET code and 
         *          cannot be introspected (i.e., you cannot look at the code and analyze it at runtime).
         *          Since the logic is compiled, frameworks like Entity Framework have no way to know 
         *          what logic is inside the function. They can't see that the logic inside your function is x => x % 2 == 0.
         * 
         *      Meanwhile, Expression<Func<T, bool>> is not compiled, so frameworks can look at the data structure (expression tree) 
         *      and figure out what the logic is, allowing them to translate it into SQL.
         *      
         * 4. Summary
         *      Func<T, bool>: A compiled delegate that runs in memory. It cannot be translated into SQL because once it is compiled,
         *      the actual logic is hidden from Entity Framework.
         *      
         *      Expression<Func<T, bool>>: A data structure that represents the logic in a way that frameworks like 
         *      Entity Framework can analyze and convert to SQL.
         *      
         * This distinction allows Expression<Func<T, bool>> to be much more powerful for querying data, as it can be translated into SQL and executed by the database, improving performance and efficiency.
         */
        #endregion
        public Expression<Func<T, bool>>? Criteria { get; set; }
        public List<Expression<Func<T, object>>> Includes { get; set; } = new List<Expression<Func<T, object>>>();
        public Expression<Func<T, object>>? OrderByAsc { get; set; }
        public Expression<Func<T, object>>? OrderByDesc { get; set; }
        public int Take { get; set; }
        public int Skip { get; set; }
        public bool IsPaginationEnabled { get; set; } = true;

        public BaseSpecification() { }
        public BaseSpecification(Expression<Func<T, bool>> Criteria)
        {
            this.Criteria = Criteria;
        }

        public void AddOrderByAsc(Expression<Func<T, object>> OrderByExpression)
        {
            OrderByAsc = OrderByExpression;
        }
        public void AddOrderByDesc(Expression<Func<T, object>> OrderByExpression)
        {
            OrderByDesc = OrderByExpression;
        }
        public void AddPagination(int take, int skip)
        {
            IsPaginationEnabled = true;
            Take = take;
            Skip = skip;
        }
    }
}
