using System.Linq.Expressions;


// Define the input data
var data = new List<Record>
        {
            new Record { Id = 1, Name = "Rashad", Age = 25 },
            new Record { Id = 2, Name = "Elvin", Age = 30 },
            new Record { Id = 3, Name = "Ceyhun", Age = 35 },
            new Record { Id = 4, Name = "Aysel", Age = 40 },
        };

// Define the filter parameters
var filters = new List<Filter>
        {
            new Filter { ColumnName = "Name", Value = "E" },
            new Filter { ColumnName = "Age", Value = "30" },
        };

// Create the parameter expression for the input data
var parameter = Expression.Parameter(typeof(Record), "record");

// Build the filter expression dynamically
Expression filterExpression = null;
foreach (var filter in filters)
{
    var property = Expression.Property(parameter, filter.ColumnName);
    var constant = Expression.Constant(filter.Value);
    Expression comparison;

    if (property.Type == typeof(string))
    {
        comparison = Expression.Call(property, "Contains", Type.EmptyTypes, constant);
    }
    else
    {
        // We need to convert value based on property type
        constant = Expression.Constant(Convert.ToInt32(filter.Value));
        comparison = Expression.Equal(property, constant);
    }

    filterExpression = filterExpression == null
        ? comparison
        : Expression.And(filterExpression, comparison);
}

// Create the lambda expression with the parameter and the filter expression
var lambda = Expression.Lambda<Func<Record, bool>>(filterExpression, parameter);

// Compile the lambda expression to create a delegate
var func = lambda.Compile();

// Filter the input data
var filteredData = data.Where(func);

// Output the filtered data
foreach (var record in filteredData)
{
    Console.WriteLine($"Id: {record.Id}, Name: {record.Name}, Age: {record.Age}");
}


class Record
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
}

class Filter
{
    public string ColumnName { get; set; }
    public string Value { get; set; }
}

