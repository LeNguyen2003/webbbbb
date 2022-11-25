using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebBanHang.Models;
using System.IO;
using PagedList;
using PagedList.Mvc;

namespace WebBanHang.Controllers
{
    public class ProductController : Controller
    {
        DBQLBHEntities db = new DBQLBHEntities();
        // GET: Product
        public ActionResult Index(string category,int?page,double min=double.MinValue, double max=double. MaxValue)
        {
            int pageSize = 4;
            int pageNum = (page ?? 1);
            if (category == null)
            {
                var productList = db.Products.OrderByDescending(x => x.NamePro);
                return View(productList.ToPagedList(pageSize, pageNum));
            }
            else
            {
                var productList = db.Products.OrderByDescending(x => x.NamePro)
                .Where(p => p.Category == category);
                return View(productList);
            }
        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }
        //product/craete
        public ActionResult Create()
        {
            var catelist = db.Category.ToList();
            ViewBag.Category1 = new SelectList(catelist, "IDCate","NameCate");
            return View();

          
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,NamePro,DecriptionPro,Category,Price,ImagePro")]Products product)
        {
            if (ModelState.IsValid)
            {
                var catelist = db.Category.ToList();
                ViewBag.Category1 = new SelectList(catelist, "IDCate", "NameCate");
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Catogory = new SelectList(db.Category, "IDCate", "NameCate", product.Category);
            return View(product);
        }
        public ActionResult ProductList(string category, int ? page, string SearchString, double min= double.MinValue, double max = double.MaxValue)
        {
            var products = db.Products.Include(p => p.Category1);
            if (category == null)
            {
                products = db.Products.OrderByDescending(x => x.NamePro);
            }
            else
            {
                products= db.Products.OrderByDescending(x=>x.Category).Where(x => x.Category == category);
            }
            if (!String.IsNullOrEmpty(SearchString))
            {
                products = products.Where(s => s.NamePro.Contains(SearchString.Trim()));
            }
            if (min >= 0 && max > 0)
            {
                products = db.Products.OrderByDescending(x => x.Price).Where(p =>
              (double)p.Price >= min && (double)p.Price <= max);
            }
            int pageSize = 4;
            int pageNumber = (page ?? 1);
            return View(products.ToPagedList(pageNumber, pageSize));

        }
        public ActionResult SelectCate()
        {
            Category slec = new Category();
            slec.ListCate = db.Category.ToList < Category >();
            return PartialView(slec);
        }
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.Category = new SelectList(db.Category, "IDCate", "NameCate", product.Category);
            return View(product);
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }
        

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Products product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Detail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,NamePro,DecriptionPro,Category,Price,ImagePro")] Products product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Category = new SelectList(db.Category, "IDCate", "NameCate", product.Category);
            return View(product);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }





    }
}