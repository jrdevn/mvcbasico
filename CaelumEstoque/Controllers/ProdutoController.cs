
using CaelumEstoque.DAO;
using CaelumEstoque.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
namespace CaelumEstoque.Controllers
{    [AutoricazaoFilter]
    public class ProdutoController : Controller
    {
        [Route("produtos", Name ="ListaProdutos")] // posso utilizar /produtos para acessar a action index da sua view..
        [AutoricazaoFilter] // irá ser chamado para executar antes de invocar essa action, no caso ela valida.. 
        //
        // GET: /Produto/

        public ActionResult Index()
        {
            object usuario = Session["usuarioLogado"];
            if(usuario != null)
            {
                ProdutosDAO dao = new ProdutosDAO();
                IList<Produto> produtos = dao.Lista();
                return View(produtos); // passa como parametro a lista de produtos.. não precisa do viewbag.. 
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
           
        }

        public ActionResult Form()
        {
            CategoriasDAO categoriasDAO = new CategoriasDAO(); // carrega o banco
            IList<CategoriaDoProduto> categorias = categoriasDAO.Lista(); // entity, recupera as categorias. 
            ViewBag.Categorias = categorias; // cria uma viewbag para ser instanciada na view.

            ViewBag.Produto = new Produto(); // carrega o produto para a viewbag da view do form (caso de erro)..

            return View();
        }

        [HttpPost] // só vai aceitar requisições Post, definido pelo HttpPost
        [ValidateAntiForgeryToken] // cria um token de validação
        public ActionResult Adiciona(Produto produto) // adiciona no bd um produto do tipo Produto, vindo da view;
        {
            int idInformatica = 1;
            if(produto.CategoriaId.Equals(idInformatica) && produto.Preco < 100 )
            {
                ModelState.AddModelError("produto.Invalido", "Informática com preço abaixo de 100 reais"); // crio minha propria modelstate e passando como parametro a mensagem de erro
                                                                                                           // e a chave do erro no primeiro parametro para passar na view
            }

            if (ModelState.IsValid)
            { // o modelo obedece as regras de validação na model E PODE SER ATRIBUIDA
                ProdutosDAO dao = new ProdutosDAO();

                dao.Adiciona(produto); // adiciona no banco

                return RedirectToAction("Index"); // manda para a controller index que irá mostrar os produtos e depois irá chamar a view
                                                  //return RedirectToAction("Index","HomeController"); para acessar a view index e da home controller. 
            }
            else
            {
                ViewBag.Produto = produto; // adicionando denovo o produto
                CategoriasDAO categoriasDAO = new CategoriasDAO();
                ViewBag.Categorias = categoriasDAO.Lista(); // PRECISO PREENCHER DENOVO AS CATEGORIAS
                return View("Form");
            }
        }

        [Route("produtos/{id}", Name="VisualizaProdutos")]  // {id} é a variavel que está passando como parametroS
        public ActionResult Visualiza(int id)
        {
            ProdutosDAO dao = new ProdutosDAO();
            Produto produto =  dao.BuscaPorId(id); // busca por id o produto que foi passado para saber suas especificações.. 
            ViewBag.Produto = produto;
            return View();
        }

        public ActionResult DecrementaQtd(int id)
        {
            ProdutosDAO dao = new ProdutosDAO();
            Produto produto = dao.BuscaPorId(id);
            dao.Atualiza(produto);
            return Json(produto); // mando como reposta o produto como parametro e faço ele virar um json
        }
    }

    internal class AutoricazaoFilterAttribute : Attribute
    {
    }
}