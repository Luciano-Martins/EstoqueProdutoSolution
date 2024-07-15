using System.Linq;
using System.Threading.Tasks;
using EstoqueWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EstoqueWeb.Controllers
{
    public class ClienteController : Controller
    {
        //crio uma propriedade privada, somente leitura, para guardar o contexto do banco de dados
        //que vem no parametro do construtor
        private readonly EstoqueWebContext _context;
        //aqui crio um construtor , vai receber context do banco
        public ClienteController(EstoqueWebContext context)
        {
            this._context = context ;
            
        }
        public async Task<IActionResult> Index()
        {
            
            return  View( await _context.Clientes.OrderBy(x => x.Nome).AsNoTracking().ToListAsync());
        }

         [HttpGet]//essa Action vai responder apenas requisições do tipo Get
        public async Task<IActionResult> Cadastrar(int? id)//vai se chamar cadastrar e opcionalmente 
        //vai receber um parametro id, se vier um parametro é alteração senão é inclusão
        {
            if (id.HasValue)//testo se tem valor , se sim 
            {
                //eu busco a Cliente atraves do context.Cliente como metodo FindAsync passando o id
                var Cliente = await _context.Clientes.FindAsync(id);
                // ai verifico e o objeto retornado é igual a null
                if (Cliente == null)
                {
                    //se for igual a null , retorno NotFound , seguinifica que não foi encontrado
                    //return NotFound();
                    TempData["mensagem"] = MensagemModel.Serializar("Cliente Não Encontrado .",TipoMensagem.Erro);
                    return RedirectToAction("Index");

                }
                //se tiver algo eu retorno a view com o objeto Cliente encontrado
                return View(Cliente);    
            }
            //agora se não possuir valor , retorno uma nova Cliente vazia , porque é uma inclusão
            return View(new ClienteModel());

        }
        //vamos criar um método para auxiliar , quando uma determinada Cliente existe ou não, apartir
        //do id dela
        private bool ClienteExiste(int id )
        {
            return _context.Clientes.Any(e => e.IdUsuario == id);
        }
        //o parametro [fromform]indica que o ClienteModel esta vindo do formulario
        [HttpPost]//ele recebe um id de forma opcional,receber os dados do form no objeto
        //ClienteModel 
        public async Task<IActionResult>Cadastrar(int? id,[FromForm]ClienteModel Cliente)
        {
            //aqui eu testo se o Model é valido, se sim entro no if 
            if (ModelState.IsValid)
            {
                // aqui eu testo se tem um id na url , se sim significa que é uma alteração,
                //senão é cadastro, então adiciono
                if (id.HasValue)
                {
                    //aqui eu testo se a categogia existe
                    if (ClienteExiste(id.Value))
                    {
                        //se  sim pego o banco e atualizo Cliente
                        _context.Clientes.Update(Cliente);
                        //aqui estou dizendo para ele não sobescrever a senha cadastrada.
                        _context.Entry(Cliente).Property(x => x.Senha).IsModified = false;

                        //aqui salvo as alterações, se conseguiu salvar , mostro a mensagem de sucesso,
                        //senão mostro a mensagem de erro
                        if (await _context.SaveChangesAsync() > 0)
                        {
                            
                            TempData["mensagem"] = 
                               MensagemModel.Serializar("Cliente alterado com sucesso.");
                        }
                        else
                        {
                            TempData["mensagem"] = 
                               MensagemModel.Serializar("Erro ao alterar Cliente",TipoMensagem.Erro);
                        }
                        
                    }
                    else
                    {
                        //se não existe mostro mensagem de erro
                        TempData["mensagem"] = MensagemModel.Serializar("Cliente não existe",TipoMensagem.Erro);

                    }
                }
                else
                {
                    //aqui faço o cadastro
                    _context.Clientes.Add(Cliente);//pego o banco e adiciono Cliente
                    //peço para adicionar , se for maior que 0 ,mostro a mensagem de cadastro com sucesso
                    if (await _context.SaveChangesAsync() > 0)
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("Cliente cadastrado com sucesso");

                    }
                    else
                    {
                        //senão mensagem de erro
                        TempData["mensagem"] = MensagemModel.Serializar("Erro ao Cadastrar Cliente", TipoMensagem.Erro);
                    }
                }
                return RedirectToAction(nameof(Index));//qualquer quwe for as mensagens será 
                //retornada para a view Index
                
            }
            else
            {
              return View(Cliente);//se ela permanece na view do formulario e mostrará os erros
            }
        }
       
       [HttpGet]
       public async Task<IActionResult>Excluir(int? id)
       {
         if (!id.HasValue)
         {
            TempData["mensagem"] = MensagemModel.Serializar("Cliente Não Informado.",
            TipoMensagem.Erro);
            return RedirectToAction(nameof(Index));
         }
         else
         {
            var Cliente = await _context.Clientes.FindAsync(id);
            if (Cliente == null)
            {
                TempData["menasgem"] = MensagemModel.Serializar("Cliente não encontrado.",
                TipoMensagem.Erro);
                return RedirectToAction(nameof(Index));
            }
           
            return View(Cliente);
         }
       }
       [HttpPost]
       public async Task<IActionResult>Excluir(int id)
       {
         var Cliente = await _context.Clientes.FindAsync(id);

         if (Cliente != null)
         {
            _context.Clientes.Remove(Cliente);
            if (await _context.SaveChangesAsync() > 0)
        
                TempData["mensagem"] = MensagemModel.Serializar("Cliente Excluido com sucesso");
            else
                TempData["mensagem"] = MensagemModel.Serializar("Não foi possivel excluir o Cliente",
                TipoMensagem.Erro);

            return RedirectToAction(nameof(Index));   
         }
         else
         {
                TempData["mensagem"] = MensagemModel.Serializar("Cliente Não Encontardo",
                    TipoMensagem.Erro);
                    return RedirectToAction(nameof(Index));
         }

           
        }
       
    }

}