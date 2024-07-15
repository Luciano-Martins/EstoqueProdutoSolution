using System.Linq;
using System.Threading.Tasks;
using EstoqueWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EstoqueWeb.Controllers
{
    public class CategoriaController : Controller
    {
        //crio uma propriedade privada, somente leitura, para guardar o contexto do banco de dados
        //que vem no parametro do construtor
        private readonly EstoqueWebContext _context;
        //aqui crio um construtor , vai receber context do banco
        public CategoriaController(EstoqueWebContext context)
        {
            this._context = context ;
            
        }
        public async Task<IActionResult> Index()
        {
            
            return  View( await _context.Categorias.OrderBy(x => x.Nome).AsNoTracking().ToListAsync());
        }

         [HttpGet]//essa Action vai responder apenas requisições do tipo Get
        public async Task<IActionResult> Cadastrar(int? id)//vai se chamar cadastrar e opcionalmente 
        //vai receber um parametro id, se vier um parametro é alteração senão é inclusão
        {
            if (id.HasValue)//testo se tem valor , se sim 
            {
                //eu busco a categoria atraves do context.categoria como metodo FindAsync passando o id
                var categoria = await _context.Categorias.FindAsync(id);
                // ai verifico e o objeto retornado é igual a null
                if (categoria == null)
                {
                    //se for igual a null , retorno NotFound , seguinifica que não foi encontrado
                    return NotFound();

                }
                //se tiver algo eu retorno a view com o objeto categoria encontrado
                return View(categoria);    
            }
            //agora se não possuir valor , retorno uma nova categoria vazia , porque é uma inclusão
            return View(new CategoriaModel());

        }
        //vamos criar um métod para auxiliar , quando uma determinada categoria existe ou não, apartir
        //do id dela
        private bool CategoriaExiste(int id )
        {
            return _context.Categorias.Any(e => e.IdCategoria == id);
        }
        //o parametro [fromform]indica que o categoriaModel esta vindo do formulario
        [HttpPost]//ele recebe um id de forma opcional,receber os dados do form no objeto
        //categoriaModel 
        public async Task<IActionResult>Cadastrar(int? id,[FromForm]CategoriaModel categoria)
        {
            //aqui eu testo se o Model é valido, se sim entro no if 
            if (ModelState.IsValid)
            {
                // aqui eu testo se tem um id na url , se sim significa que é uma alteração,
                //senão é cadastro, então adiciono
                if (id.HasValue)
                {
                    //aqui eu testo se a categogia existe
                    if (CategoriaExiste(id.Value))
                    {
                        //se  sim pego o banco e atualizo categoria
                        _context.Categorias.Update(categoria);

                        //aqui salvo as alterações, se conseguiu salvar , mostro a mensagem de sucesso,
                        //senão mostro a mensagem de erro
                        if (await _context.SaveChangesAsync() > 0)
                        {
                            
                            TempData["mensagem"] = 
                               MensagemModel.Serializar("Categoria alterada com sucesso.");
                        }
                        else
                        {
                            TempData["mensagem"] = 
                               MensagemModel.Serializar("Erro ao alterar categoria",TipoMensagem.Erro);
                        }
                        
                    }
                    else
                    {
                        //se não existe mostro mensagem de erro
                        TempData["mensagem"] = MensagemModel.Serializar("Categoria não existe",TipoMensagem.Erro);

                    }
                }
                else
                {
                    //aqui faço o cadastro
                    _context.Categorias.Add(categoria);//pego o banco e adiciono Categoria
                    //peço para adicionar , se for maior que 0 ,mostro a mensagem de cadastro com sucesso
                    if (await _context.SaveChangesAsync() > 0)
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("Categoria cadastrada com sucesso");

                    }
                    else
                    {
                        //senão mensagem de erro
                        TempData["mensagem"] = MensagemModel.Serializar("Erro ao Cadastrar categoria", TipoMensagem.Erro);
                    }
                }
                return RedirectToAction(nameof(Index));//qualquer quwe for as mensagens será 
                //retornada para a view Index
                
            }
            else
            {
              return View(categoria);//se ela permanece na view do formulario e mostrará os erros
            }
           
           

        }
       


       [HttpGet]
       public async Task<IActionResult>Excluir(int? id)
       {
         if (!id.HasValue)
         {
            TempData["mensagem"] = MensagemModel.Serializar("Categoria Não Informada.",
            TipoMensagem.Erro);
            return RedirectToAction(nameof(Index));
         }
         else
         {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null)
            {
                TempData["menasgem"] = MensagemModel.Serializar("Categoria não encontrada.",
                TipoMensagem.Erro);
                return RedirectToAction(nameof(Index));
            }
           
            return View(categoria);
         }
       }
       [HttpPost]
       public async Task<IActionResult>Excluir(int id)
       {
         var categoria = await _context.Categorias.FindAsync(id);

         if (categoria != null)
         {
            _context.Categorias.Remove(categoria);
            if (await _context.SaveChangesAsync() > 0)
        
                TempData["mensagem"] = MensagemModel.Serializar("Categoria Excluida com sucesso");
            else
                TempData["mensagem"] = MensagemModel.Serializar("Não foi possivel excluir a categoria",
                TipoMensagem.Erro);

            return RedirectToAction(nameof(Index));   
         }
         else
         {
                TempData["mensagem"] = MensagemModel.Serializar("cATEGORIA NÃO ENCONTRADA",
                    TipoMensagem.Erro);
                    return RedirectToAction(nameof(Index));
         }

           
        }
       
    }

}