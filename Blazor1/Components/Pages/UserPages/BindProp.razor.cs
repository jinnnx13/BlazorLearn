
using Blazor1.Models;

namespace Blazor1.Components.Pages.UserPages
{
    public partial class BindProp
    {

        public string SelectedProp = " ";
        Models.Product Products = new()
        {
            Id = 1,
            Name = "Product-1",
            IsActive = true,
            price = 5,
            ProductProperties = new List<Models.ProductPropModel>()
            {
                new ProductPropModel{ Id=1, Key="Color", Value="Red" },
                new ProductPropModel{ Id=2, Key="Size", Value="20Oz" },
                new ProductPropModel{ Id=3, Key="Flavor", Value="Rose" },
            }
        };
        List<Product> ProductList = new();

        protected override void OnInitialized()
        {
            ProductList.Add(new()
            {
                Id = 1,
                Name = "Midnight Blaze",
                IsActive = false,
                price = 5.99,
                ProductProperties = new List<ProductPropModel>()
                {
                    new ProductPropModel{ Id=1, Key="Color", Value="Red" },
                    new ProductPropModel{ Id=2, Key="Size", Value="20Oz" },
                    new ProductPropModel{ Id=3, Key="Flavor", Value="Rose" },
                }
            });
            ProductList.Add(new()
            {
                Id = 2,
                Name = "Blossom Lily",
                IsActive = true,
                price = 10.50,
                ProductProperties = new List<ProductPropModel>()
                {
                    new ProductPropModel{ Id=1, Key="Flavor", Value="Lily" },
                    new ProductPropModel{ Id=2, Key="Size", Value="18Oz" },
                    new ProductPropModel{ Id=3, Key="Color", Value="White" },
                }
            });

        }


        private string show = " ";

        private void Show()
        {
            show = "text";
        }
    }
}
