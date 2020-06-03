using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace DesignPatterns
{
    public enum Color
    {
        Red, 
        Blue, 
        Yellow
    }
    
    public enum Size
    {
        Small, 
        Medium, 
        Large
    }

    public class Product
    {
        public string Name;
        public Color Color;
        public Size Size;

        public Product(string name, Color color, Size size)
        {
            Name = name;
            Color = color;
            Size = size;
        }
    }

    public class ProductFilter
    {
        public IEnumerable<Product> FilterBySize(IEnumerable<Product> products, Size size)
        {
            return products.Where(product => product.Size == size);
        }

        public IEnumerable<Product> FilterByColor(IEnumerable<Product> products, Color color)
        {
            return products.Where(product => product.Color == color);
        }
    }

    public interface ISpecification<T>
    {
        bool IsSatisfied(T type);
    }    
    
    public interface IFilter<T>
    {
        IEnumerable<T> Filter(IEnumerable<T> items, ISpecification<T> specification);
    }

    public class ColorSpecification : ISpecification<Product>
    {
        private Color color;
        
        public ColorSpecification(Color color)
        {
            this.color = color;
        }
        public bool IsSatisfied(Product type)
        {
            return type.Color == color;
        }
    }

    public class SizeSpecification : ISpecification<Product>
    {
        private Size size;

        public SizeSpecification(Size size)
        {
            this.size = size;
        }
        
        public bool IsSatisfied(Product type)
        {
            return type.Size == size;
        }
    }

    public class AndSpecification<T> : ISpecification<T>
    {
        private readonly ISpecification<T> first, second;

        public AndSpecification(ISpecification<T> first, ISpecification<T> second)
        {
            this.first = first;
            this.second = second;
        }
        
        public bool IsSatisfied(T type)
        {
            return first.IsSatisfied(type) && second.IsSatisfied(type);
        }
    }
    
    public class BetterFilter : IFilter<Product>
    {
        public IEnumerable<Product> Filter(IEnumerable<Product> items, ISpecification<Product> specification)
        {
            return items.Where(item => specification.IsSatisfied(item));
        }
    }
    
    
    public static class Program
    {
        private static void Main(string[] args)
        {
            var apple = new Product("Apple", Color.Blue, Size.Small);
            var tree = new Product("Tree", Color.Red, Size.Large);
            var house = new Product("House", Color.Yellow, Size.Large);

            Product[] products = {apple, tree, house};
            var productFilter = new ProductFilter();
            
            Console.WriteLine("Red products (old method): ");
            foreach (var product in productFilter.FilterByColor(products, Color.Red))
            {
                Console.WriteLine($" - {product.Name} is red");
            }
            
            var betterFilter = new BetterFilter();
            Console.WriteLine("Green products (new): ");
            foreach (var  product in betterFilter.Filter(products, new ColorSpecification(Color.Red)))
            {
                Console.WriteLine($" - {product.Name} is red");
            }            
            
            Console.WriteLine("Large yellow items: ");
            foreach (var product in betterFilter.Filter(products, 
                new AndSpecification<Product>(new ColorSpecification(Color.Yellow), new SizeSpecification(Size.Large))
                )
            )
            {
                Console.WriteLine($" - {product.Name} is large and yellow");
            }
        }
    }
}