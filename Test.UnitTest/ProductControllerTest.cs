using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Web;
using Test.Web.Controllers;
using Test.Web.Repositories;
using Xunit;

namespace Test.UnitTest
{
    public class ProductControllerTest
    {
        private Mock<IRepository<Produuct>> _mock;
        private ProduuctsController _controller;
        private List<Produuct> _produucts; //mock datalar olsun dedik buna.

        public ProductControllerTest()
        {
            _mock = new Mock<IRepository<Produuct>>();
            _controller = new ProduuctsController(_mock.Object);
            _produucts = new List<Produuct>()
            {
                new Produuct()
                {
                    Id = 1,
                    Name="KalemGitDeneme",
                    Stock=100,
                    Color="Red",
                    Price=100,
                },
                new Produuct()
                {
                    Id = 2,
                    Name="Kalemkutusu",
                    Stock=200,
                    Color="Blue",
                    Price=50,
                },
                new Produuct()
                {
                    Id = 3,
                    Name="Silgi",
                    Stock=10,
                    Color="White",
                    Price=90,
                }
            };
            
        }
        [Fact]
        public async void Index_ActionExecute_ReturnView()
        {
            var result = await _controller.Index();
            Assert.IsType<ViewResult>(result);

        }
        [Fact]
        public async void Index_ActionExecute_ReturnProducts()
        {
            var myMock = _mock.Setup(x => x.GetAll()).ReturnsAsync(_produucts);           
            var result = await _controller.Index();

            var viewResult=Assert.IsType<ViewResult>(result); //viewresultu kontrol ettik.
            var ProductList = Assert.IsAssignableFrom<IEnumerable<Produuct>>(viewResult.Model); // daha sonra model geliyor mu'ya baktık.
            Assert.Equal<int>(3, _produucts.Count); //burada da 2 tane product bekliyorduk ona baktık.          

        }
        [Fact]
        public async void Details_IdNull_RedirectToIndex()
        {
            var result = await _controller.Details(null);
            var redirect= Assert.IsType<RedirectToActionResult>(result); //redirect oldu mu ?

            Assert.Equal("Index", redirect.ActionName); // peki index'e mi döndü check.
        }
        [Fact]
        public async void Details_IdInvalid_ReturnNotFound()
        {
            Produuct product=null;
            _mock.Setup(x => x.GetById(0)).ReturnsAsync(product);
            var result= await _controller.Details(0);

            var redirect = Assert.IsType<NotFoundResult>(result);

            Assert.Equal<int>(404, redirect.StatusCode);

        }
        [Theory]
        [InlineData(1)]
        public async void Details_IdValid_ReturnProduct(int productId)
        {
            Produuct produuct = _produucts.First(x=>x.Id==productId);
            _mock.Setup(x=>x.GetById(productId)).ReturnsAsync(produuct);

            var result = await _controller.Details(productId);
            var isViewResult=Assert.IsType<ViewResult>(result);

            var resultProduct = Assert.IsAssignableFrom<Produuct>(isViewResult.Model);

            Assert.Equal(produuct.Id, resultProduct.Id);
            Assert.Equal(produuct.Name, resultProduct.Name);


        }

        [Fact]
        public void Create_ActionExecute_ReturnView()
        {
            var result =  _controller.Create();
            var isView=Assert.IsType<ViewResult>(result);
        }
        [Fact]
        public async void CreatePOST_InvalidModel_ReturnView()
        {
            _controller.ModelState.AddModelError("Name", "Name is required"); //modelstate valid olmıycak ve view'e döncez, modeli product.

            var result=await _controller.Create(_produucts.Where(x=>x.Id==1).FirstOrDefault());

            var isView= Assert.IsType<ViewResult>(result);
            var isModel = Assert.IsAssignableFrom<Produuct>(isView.Model);

        }

        [Fact]
        public async void CreatePOST_ValidModel_RedirectToIndex()
        {
            var result = await _controller.Create(_produucts.First()); // bize çıktı önemli burayı salladık bi tane datayla. mock'a da gerek yok metod bizi ilgilendirmiyo
            var isRedirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", isRedirect.ActionName);
            
        }

        [Fact]
        public async void CreatePOST_ValidModel_CreateMethodExecute()
        {
            Produuct product = null;
            _mock.Setup(x => x.Create(It.IsAny<Produuct>())).Callback<Produuct>(x => product = x); //callback tekrar calıstırılıcak method.
            //aşağıdaki satır ile çakma methodumuza listedeki ilk elemanı verdik ve product nesnesine eşitledik.

            var result = await _controller.Create(_produucts.First());
            _mock.Verify(x=>x.Create(It.IsAny<Produuct>()),Times.Once); //create 1 kere çalıştı mı ?

            Assert.Equal(_produucts.First().Id, product.Id); //eklenenle eklemeye calıstıgımız aynı mı bakalım , eklemiş miyiz anlamak icin

        }

        [Fact]
        public async void CreatePOST_InvalidModel_DontExecuteCreate()
        {
            _controller.ModelState.AddModelError("Name", "Name is required"); // modelstate'e hata veriyoruz. girmemesi lazım bize.
            var result = await _controller.Create(_produucts.First());
            _mock.Verify(x=>x.Create(_produucts.First()),Times.Never);
        }

        [Fact]
        public async void Edit_IdNull_RedirectToIndex()
        {
            var result = await _controller.Edit(null);
            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirect.ActionName);

        }
        [Theory]
        [InlineData(3)]
        public async void Edit_IdInvalid_ReturnNotFound(int id)
        {
            Produuct product = null;
            _mock.Setup(x => x.GetById(id)).ReturnsAsync(product);
            var result = await _controller.Edit(id);

            var redirect=Assert.IsType<NotFoundResult>(result);
            Assert.Equal<int>(404, redirect.StatusCode); // notfound'la birlikte statuscode'u 404 bekliyoruz.
        }

        [Theory]
        [InlineData(2)]
        public async void Edit_IdValid_ReturnViewModel(int id)
        {
            Produuct product = _produucts.FirstOrDefault(x=>x.Id== id);
            _mock.Setup(x => x.GetById(id)).ReturnsAsync(product);

            var result= await _controller.Edit(id);
            var isView = Assert.IsType<ViewResult>(result);

            var resultProduct = Assert.IsAssignableFrom<Produuct>(isView.Model); //birbirine atanıp atanamıycağını karşılaştırır.

            Assert.Equal(product.Id, resultProduct.Id);
            Assert.Equal(product.Name, resultProduct.Name);


        }

        [Theory]
        [InlineData(1)]
        public void ProductExists_AllSitations_ReturnBool(int id)
        {
            Produuct nullProduct = null;
            Produuct validProduct = _produucts.First(x => x.Id == id);
            
            _mock.Setup(x=>x.GetById(id)).ReturnsAsync(nullProduct);
            var result =  _controller.ProduuctExists(id);
            Assert.False(result);

            _mock.Setup(x => x.GetById(id)).ReturnsAsync(validProduct);
            var resultSecond =  _controller.ProduuctExists(id);
            Assert.True(resultSecond);

        }




    }
}
