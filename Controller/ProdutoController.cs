using System.Linq;
using System.Threading.Tasks;
using EstoqueWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EstoqueWeb.Controllers
{
    public class ProdutoController : Controller
    {
        //crio uma propriedade privada, somente leitura, para guardar o contexto do banco de dados
        //que vem no parametro do construtor
        private readonly EstoqueWebContext _context;
        //aqui crio um construtor , vai receber context do banco
        public ProdutoController(EstoqueWebContext context)
        {
            this._context = context;

        }
        public async Task<IActionResult> Index()
        {
            //aqui sempre que eu carrego os produtos , quero mostar tambem a categoria, o método include
            //faz isso para mim, ou seja , carrego o Produto e a categoria associada a ele.

            return View(await _context.Produtos.OrderBy(x => x.Nome).Include(x => x.Categoria).AsNoTracking().ToListAsync());
        }

        [HttpGet]//essa Action vai responder apenas requisições do tipo Get
        public async Task<IActionResult> Cadastrar(int? id)//vai se chamar cadastrar e opcionalmente 
        //vai receber um parametro id, se vier um parametro é alteração senão é inclusão
        {
            //sempre que for cadastrar o produto, vai ter um campo para o usuário
            // selecionar a categoria que o produto pertence de alguma forma,
            //tenho que passar uma coleção de categorias para o formulario de cadastro de produtos
            //vou passar atraves da ViewBag
            //Vou começar trazendo todas as categoria do banco
            var categorias = _context.Categorias.OrderBy(x => x.Nome).AsNoTracking().ToList();
            //agora vou criar uma coleção especial SelectList usada para preencher opções do elemento 
            //select na View ,para cria um novo selectList, tenho que passar a font de dados nesse 
            //caso é categorias,qual a propriedade que será usada para valor de cada opção,(CategoriaModel.IdCategoria)
            //o valor vai ser Id ,e o Nome
            var categoriaSelectList = new SelectList(categorias,
            nameof(CategoriaModel.IdCategoria),nameof(CategoriaModel.Nome));
            //agora vamos guardar na ViewBag
            ViewBag.Categorias = categoriaSelectList;


            if (id.HasValue)//testo se tem valor , se sim 
            {
                //eu busco o produto atraves do context.produto com o método FindAsync passando o id
                var produto = await _context.Produtos.FindAsync(id);
                // ai verifico e o objeto retornado é igual a null
                if (produto == null)
                {
                    //se for igual a null , retorno NotFound , significa que não foi encontrado.
                    return NotFound();

                }
                //se tiver algo eu retorno a view com o objeto produto encontrado
                return View(produto);
            }
            //agora se não possuir valor , retorno uma novo produto vazio, porque vai fazer uma inclusão
            return View(new ProdutoModel());

        }
        //vamos criar um método para auxiliar , quando um determinado produto existe ou não, apartir
        //do id dele
        private bool ProdutoExiste(int id)
        {
            return _context.Produtos.Any(e => e.IdProduto == id);
        }
        //o parametro [fromform]indica que o produtoModel esta vindo do formulario
        [HttpPost]//ele recebe um id de forma opcional,receber os dados do form no objeto
        //produtoModel 
        public async Task<IActionResult> Cadastrar(int? id, [FromForm] ProdutoModel produto)
        {
            //aqui eu testo se o Model é valido, se sim entro no if 
            if (ModelState.IsValid)
            {
                // aqui eu testo se tem um id na url , se sim, significa que é uma alteração,
                //senão é cadastro, então adiciono
                if (id.HasValue)
                {
                    //aqui eu testo se a categogia existe
                    if (ProdutoExiste(id.Value))
                    {
                        //se sim,pego o banco e atualizo produto
                        _context.Produtos.Update(produto);

                        //aqui salvo as alterações, se conseguiu salvar , mostro a mensagem de sucesso,
                        //senão mostro a mensagem de erro
                        if (await _context.SaveChangesAsync() > 0)
                        {

                            TempData["mensagem"] =
                               MensagemModel.Serializar("Produto alterada com sucesso.");
                        }
                        else
                        {
                            TempData["mensagem"] =
                               MensagemModel.Serializar("Erro ao alterar produto", TipoMensagem.Erro);
                        }

                    }
                    else
                    {
                        //se não existe mostro mensagem de erro
                        TempData["mensagem"] = MensagemModel.Serializar("Produto não existe", TipoMensagem.Erro);

                    }
                }
                else
                {
                    //aqui faço o cadastro
                    _context.Produtos.Add(produto);//pego o banco e adiciono Produto
                    //peço para adicionar , se for maior que 0 ,mostro a mensagem de cadastro com sucesso
                    if (await _context.SaveChangesAsync() > 0)
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("Produto cadastrado com sucesso");

                    }
                    else
                    {
                        //senão mensagem de erro
                        TempData["mensagem"] = MensagemModel.Serializar("Erro ao Cadastrar produto", TipoMensagem.Erro);
                    }
                }
                return RedirectToAction(nameof(Index));//qualquer que for as mensagens será 
                                                       //retornada para a view Index

            }
            else
            {
                return View(produto);//se ela permanece na view do formulario e mostrará os erros
            }
        }
        [HttpGet]
        public async Task<IActionResult> Excluir(int? id)
        {
            if (!id.HasValue)
            {
                TempData["mensagem"] = MensagemModel.Serializar("Produto Não Informado.",
                TipoMensagem.Erro);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var produto = await _context.Produtos.FindAsync(id);
                if (produto == null)
                {
                    TempData["menasgem"] = MensagemModel.Serializar("Produto não encontrado.",
                    TipoMensagem.Erro);
                    return RedirectToAction(nameof(Index));
                }

                return View(produto);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Excluir(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);

            if (produto != null)
            {
                _context.Produtos.Remove(produto);
                if (await _context.SaveChangesAsync() > 0)

                    TempData["mensagem"] = MensagemModel.Serializar("Produto Excluido com sucesso");
                else
                    TempData["mensagem"] = MensagemModel.Serializar("Não foi possivel excluir a produto",
                    TipoMensagem.Erro);

            
            }
            else
            {
                TempData["mensagem"] = MensagemModel.Serializar("Produto não encontrado",
                    TipoMensagem.Erro);
              
            }


            return RedirectToAction(nameof(Index));

        }

    }

}