using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Shopbridge_base.Data;
using Shopbridge_base.Domain.Models;
using Shopbridge_base.Domain.Services.Interfaces;
using Xceed.Wpf.Toolkit;

namespace Shopbridge_base.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService productService;
        private readonly ILogger<ProductsController> logger;
        MySql.Data.MySqlClient.MySqlConnection conn;
        string myConnectionString = "server=localhost;uid=root;" +
    "pwd=root;database=shopbridge";
        //myConnectionString = "server=localhost;uid=root;" +
        //"pwd=null;database=testdb";
public ProductsController(IProductService _productService)
        {
            this.productService = _productService;
            try
            {
                conn = new MySql.Data.MySqlClient.MySqlConnection();
                conn.ConnectionString = myConnectionString;
                conn.Open();
                Console.WriteLine("Connected!!");
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

       
        [HttpGet]   
        public async Task<ActionResult<IEnumerable<Product>>> GetProduct()
        {
            Console.WriteLine("Get1!!");
            string sql = "SELECT * FROM items";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            using (MySqlDataReader rdr = cmd.ExecuteReader())
            {
                List<Product> prod=new List<Product>();
                while (rdr.Read())
                {
                    Product temp=new Product(); temp.Product_Id=rdr.GetInt32(0);
                    if( !Convert.IsDBNull(rdr["name"]))
                        temp.Name = rdr.GetString(1);

                    Console.WriteLine("{0}", rdr.GetInt32(0));
                    prod.Add(temp);
                    /* iterate once per row */
                }
                rdr.Close();
                return prod.ToArray();
            }
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            Console.WriteLine("Get2!!");
            string sql = "SELECT * FROM items where id =" +id;
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            Product prod = new Product();
            using (MySqlDataReader rdr = cmd.ExecuteReader())
            {
                
                while (rdr.Read())
                {
                    /*Product temp = new Product();*/ prod.Product_Id = rdr.GetInt32(0);
                    prod.Name = rdr.GetString(1);
                    Console.WriteLine("{0}, {1}", rdr.GetInt32(0),rdr.GetString(1));
                    //prod.Add(temp);
                    /* iterate once per row */
                }
                rdr.Close();
                
            }
            return prod;
            //return NoContent();
        }

       
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            Console.WriteLine("Put!!");
            string sql = "UPDATE items SET id="+product.Product_Id+ ", name='"+product.Name+"' WHERE id="+ product.Product_Id;
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            //Product prod = new Product();
            int result = cmd.ExecuteNonQuery();
            Console.WriteLine("put result is: ",sql);
            Console.WriteLine(sql);
            Console.WriteLine(result);
            Console.WriteLine("end!!");

            //return result;
            //return product;
            return NoContent();
        }

        
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            Console.WriteLine("Post!!");
            string sql = "INSERT INTO items(id,name) VALUES(" + product.Product_Id+",'"+product.Name+"')";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            //Product prod = new Product();
            int result = cmd.ExecuteNonQuery();
            
            return product;
            //return NoContent();
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {

            Console.WriteLine("Delete!!");
            string sql = "DELETE FROM items where id="+id;
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            //Product prod = new Product();
            int result = cmd.ExecuteNonQuery();

            //return product;
            return NoContent();
        }

        private bool ProductExists(int id)
        {
            string sql = "if exists(select * from items where id=" + id + ")";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            //Product prod = new Product();
            int result = cmd.ExecuteNonQuery();
            if(result == 0 )
            return false;
            return true;
        }
    }
}
