using System;
using System.Linq;
using System.Threading.Tasks;
using EstoqueWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EstoqueWeb.Controllers
{
    public class PedidoController : Controller
    {
        //crio uma propriedade privada, somente leitura, para guardar o contexto do banco de dados
        //que vem no parametro do construtor
        private readonly EstoqueWebContext _context;
        //aqui crio um construtor , vai receber context do banco
        public PedidoController(EstoqueWebContext context)
        {
            this._context = context ;
            
        }

        /* na Pratica , esse método esta pegando todos os pedidos de um determinado cliente,
        ordenando esses pedidos de maneira Descendente pelo id do pedido ,isso vai fazer que o pedido
         mais recente seja mostrado primeiro*/
        public async Task<IActionResult> Index( int? cid)
        {
            //primeiro testo se o id tem um valor
            if (cid.HasValue)
            {
                // se sim , capturo esse id no banco e atribuo a variavel
                var cliente = await _context.Clientes.FindAsync(cid);
                //agora eu testo para ver se é diferente de null
                if (cliente != null)
                {
                    //agora aqui eu pego os pedidos desse cliente e ordeno de forma descendente através
                    // do método OrderByDescending ,utilizando o id do cliente
                    // de uma forma não rastreada( AsNoTracking)
                    var  pedidos = await _context.Pedidos.Where(p => p.IdCliente == cid)
                    .OrderByDescending(x => x.IdPedido).AsNoTracking().ToListAsync();
                    //agora vou guardar na ViewBag
                    ViewBag.Cliente = cliente;
                    //e retorno para View pedidos o cliente que acabei de capturar
                    return View(pedidos);
                }
                else
                {
                    // se for nulo retorno a mensagem de erro.
                    TempData["mensagem"] = MensagemModel.Serializar("Cliente não encontrado", TipoMensagem.Erro);
                    return RedirectToAction("Index", "Cliente");
                }   
            }
            else
            {
                //se o id não tem valor , mostro  a mensagem
                TempData["mensagem"] = MensagemModel.Serializar("Cliente não Informado", TipoMensagem.Erro);
                return RedirectToAction("Index", "Cliente");  
            }
       }

        [HttpGet]//essa Action vai responder apenas requisições do tipo Get

        public async Task<IActionResult> Cadastrar(int? cid)
        {
           //testo se idCliente tem um id 
           if (cid.HasValue)
           {
                //se cliente tiver valor, vou capturar o valor dele através do id
                var cliente = await _context.Clientes.FindAsync(cid);
                //vou testar se é diferente de null, se sim,vou carregar os pedidos desse cliente,
                if(cliente != null)
                {
                    // nesse momento eu carreguei todos os pedidos desse cliente
                    _context.Entry(cliente).Collection( c => c.Pedidos).Load();
                    // crio essa variabel tipo pedidoModel , é agora que vou verificar  se tem 
                    //pedido em aberto.
                    PedidoModel pedido = null;
                      //agora eu testo se IdCliente é == cid e DataPedido não tem um valor
                      //se sim significa que pedido esta em aberto, pois a data só é gravada no pedido
                      //quando ele esta fechado
                    if (_context.Pedidos.Any(p => p.IdCliente == cid && !p.DataPedido.HasValue))
                    {
                       //sendo assim , faço a variavel receber o pedido
                       //estou pegando o cliente que esteje com data de pedido em aberto
                       pedido = await _context.Pedidos.FirstOrDefaultAsync
                       (p => p.IdCliente == cid && !p.DataPedido.HasValue); 
                    }
                    else
                    {
                        //caso contrario , vamos criar um novo pedido
                        pedido = new PedidoModel { IdCliente = cid.Value, ValorTotal = 0 };
                        //aqui eu adiciono o pedido
                        cliente.Pedidos.Add(pedido);
                        //e salvo o pedido no banco
                        await _context.SaveChangesAsync();
                    }
                    // vou redirecionar a ação para Index do ItemPedido, passando para essa ação
                    //via rota , idPedido
                    return RedirectToAction("Index","ItemPedido", new {ped = pedido.IdPedido});
                  
                }
                 TempData["mensagem"] = MensagemModel.Serializar("Cliente não encontrado",TipoMensagem.Erro);
                 return RedirectToAction("Index" , "Cliente");
           }
            TempData["mensagem"] = MensagemModel.Serializar("Cliente não Informado", TipoMensagem.Erro);
            return RedirectToAction("Index", "Cliente");

        }
        //vamos criar um métod para auxiliar , quando uma determinada Pedido existe ou não, apartir
        //do id dela
        private bool PedidoExiste(int id )
        {
            return _context.Pedidos.Any(e => e.IdPedido == id);
        }
        //o parametro [fromform]indica que o PedidoModel esta vindo do formulario
        [HttpPost]//ele recebe um id de forma opcional,receber os dados do form no objeto
        //PedidoModel 
        public async Task<IActionResult>Cadastrar(int? id,[FromForm]PedidoModel Pedido)
        {
            //aqui eu testo se o Model é valido, se sim entro no if 
            if (ModelState.IsValid)
            {
                // aqui eu testo se tem um id na url , se sim significa que é uma alteração,
                //senão é cadastro, então adiciono
                if (id.HasValue)
                {
                    //aqui eu testo se a categogia existe
                    if (PedidoExiste(id.Value))
                    {
                        //se  sim pego o banco e atualizo Pedido
                        _context.Pedidos.Update(Pedido);

                        //aqui salvo as alterações, se conseguiu salvar , mostro a mensagem de sucesso,
                        //senão mostro a mensagem de erro
                        if (await _context.SaveChangesAsync() > 0)
                        {
                            
                            TempData["mensagem"] = 
                               MensagemModel.Serializar("Pedido alterada com sucesso.");
                        }
                        else
                        {
                            TempData["mensagem"] = 
                               MensagemModel.Serializar("Erro ao alterar Pedido",TipoMensagem.Erro);
                        }
                        
                    }
                    else
                    {
                        //se não existe mostro mensagem de erro
                        TempData["mensagem"] = MensagemModel.Serializar("Pedido não existe",TipoMensagem.Erro);

                    }
                }
                else
                {
                    //aqui faço o cadastro
                    _context.Pedidos.Add(Pedido);//pego o banco e adiciono Pedido
                    //peço para adicionar , se for maior que 0 ,mostro a mensagem de cadastro com sucesso
                    if (await _context.SaveChangesAsync() > 0)
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("Pedido cadastrada com sucesso");

                    }
                    else
                    {
                        //senão mensagem de erro
                        TempData["mensagem"] = MensagemModel.Serializar("Erro ao Cadastrar Pedido", TipoMensagem.Erro);
                    }
                }
                return RedirectToAction(nameof(Index));//qualquer quwe for as mensagens será 
                //retornada para a view Index
                
            }
            else
            {
              return View(Pedido);//se ela permanece na view do formulario e mostrará os erros
            }
        }
       


       [HttpGet]
       public async Task<IActionResult>Excluir(int? id)
       {
         if (!id.HasValue)
         {
            TempData["mensagem"] = MensagemModel.Serializar("Pedido Não Informado.",
            TipoMensagem.Erro);
            return RedirectToAction("Index");
         }
         if (!PedidoExiste(id.Value))
         {
                TempData["menasgem"] = MensagemModel.Serializar("Pedido não encontrado.",
                   TipoMensagem.Erro);
                return RedirectToAction("Index","Cliente");
            
         }
         //estou aqui pedando todas as iformação do pedido, porque o usuario tenha certeza do pedido 
         //que esta excluindo
            var pedido = await _context.Pedidos
            .Include(p => p.Cliente)
            .Include(p => p.ItensPedido)
            .ThenInclude(i => i.Produto)
            .FirstOrDefaultAsync(p => p.IdPedido == id);
            return View(pedido);
       }


       [HttpPost]
       public async Task<IActionResult>Excluir(int id)
       {
         var Pedido = await _context.Pedidos.FindAsync(id);

         if (Pedido != null)
         {
            _context.Pedidos.Remove(Pedido);
            if (await _context.SaveChangesAsync() > 0)
        
                TempData["mensagem"] = MensagemModel.Serializar("Pedido Excluida com sucesso");
            else
                TempData["mensagem"] = MensagemModel.Serializar("Não foi possivel excluir a Pedido",
                TipoMensagem.Erro);

            return RedirectToAction(nameof(Index), new {cid = Pedido.IdCliente});   
         }
         else
         {
                TempData["mensagem"] = MensagemModel.Serializar("Pedido Não encontrado",
                    TipoMensagem.Erro);
                    return RedirectToAction(nameof(Index),"Cliente");
         }

           
        }



        [HttpGet]
        public async Task<IActionResult> Fechar(int? id)
        {
            if (!id.HasValue)
            {
                TempData["mensagem"] = MensagemModel.Serializar("Pedido Não Informado.",
                TipoMensagem.Erro);
                return RedirectToAction("Index");
            }
            if (!PedidoExiste(id.Value))
            {
                TempData["menasgem"] = MensagemModel.Serializar("Pedido não encontrado.",
                   TipoMensagem.Erro);
                return RedirectToAction("Index", "Cliente");

            }
            //estou aqui pedando todas as iformação do pedido, porque o usuario tenha certeza do pedido 
            //que esta excluindo
            var pedido = await _context.Pedidos
            .Include(p => p.Cliente)
            .Include(p => p.ItensPedido)
            .ThenInclude(i => i.Produto)
            .FirstOrDefaultAsync(p => p.IdPedido == id);
            return View(pedido);
        }


        [HttpPost]
        public async Task<IActionResult> Fechar(int id)
        {
             // primeiro verifico se pedido existe
            if (PedidoExiste(id))
            {
                   //agora verifico se id é igual idPedido e carrego na variavel pedido
                   //incluindo produtos de item de pedido
                    var pedido = await _context.Pedidos
                    .Include(p => p.Cliente)
                    .Include(p => p.ItensPedido)
                    .ThenInclude(i => i.Produto)
                    .FirstOrDefaultAsync(p => p.IdPedido == id);
               //agora testo se pedido é maior que 0
                if (pedido.ItensPedido.Count() > 0)
                {
                    //se sim faço pedido receber data atual
                    //para fechar um pedido preciso adicionar a data Fechamento dele
                    //pego a data do pedido e adiciono a data atual
                    pedido.DataPedido  = DateTime.Now; 
                    //agora preciso fazer dea baixa no estoque, vamos fazer
                    //debito do estoque e salvo as alterações 
                    foreach (var item in pedido.ItensPedido)
                    item.Produto.Estoque -= item.Quantidade; 
                    //agora preciso salvar a alteração
                    if (await _context.SaveChangesAsync() > 0 )
                    TempData["mensagem"] = MensagemModel.Serializar("Pedido Fechado com sucesso");
                    else
                    TempData["mensagem"] = MensagemModel.Serializar("Não foi possivel fechar o pedido",
                    TipoMensagem.Erro);
                    //agora vou redirecionar para a ação Index passando a id cliente
                    return RedirectToAction("Index", new {cid = pedido.IdCliente});  
                }
                else
                {
                    TempData["mensagem"] = MensagemModel.Serializar("Não é possivel fechar um pedido sem Itens.",
                    TipoMensagem.Erro);
                    return RedirectToAction("Index", new { cid = pedido.IdCliente });
                }
                
            }
            else
            {
                TempData["mensagem"] = MensagemModel.Serializar("Pedido não encontrado",
                     TipoMensagem.Erro);
                return RedirectToAction("Index", "Cliente");
            }

            


        }



        [HttpGet]
        public async Task<IActionResult> Entregar(int? id)
        {
            if (!id.HasValue)
            {
                TempData["mensagem"] = MensagemModel.Serializar("Pedido Não Informado.",
                TipoMensagem.Erro);
                return RedirectToAction("Index");
            }
            if (!PedidoExiste(id.Value))
            {
                TempData["menasgem"] = MensagemModel.Serializar("Pedido não encontrado.",
                   TipoMensagem.Erro);
                return RedirectToAction("Index", "Cliente");

            }
            //estou aqui pedando todas as iformação do pedido, porque o usuario tenha certeza do pedido 
            //que esta excluindo
            var pedido = await _context.Pedidos
            .Include(p => p.Cliente)
            .ThenInclude(c => c.Enderecos)
            .Include(p => p.ItensPedido)
            .ThenInclude(i => i.Produto)
            .FirstOrDefaultAsync(p => p.IdPedido == id);
            return View(pedido);
        }


        [HttpPost]
        public async Task<IActionResult> Entregar(int idPedido, int idEndereco)
        {
           //esse método recebe idPedido e idEndereco
           //testo se o pedido existe
            if (PedidoExiste(idPedido))
            { 
                //se sim eu carrego esse pedido ,junto com o cliente e endereços dele
                var pedido = await _context.Pedidos
                .Include(p => p.Cliente)
                .ThenInclude(c => c.Enderecos)
                .FirstOrDefaultAsync(p => p.IdPedido == idPedido);
                //agora eu capturo o pedido jumto com o cliente e os endereços dele passando o 
                //comparando o idendereco com oendereço vindo no parametro
                 var endereco = pedido.Cliente.Enderecos
                 .FirstOrDefault(e => e.IdEndereco == idEndereco);
                 //agora que tenho o endereço , verifico se é diferente de nulo
                if (endereco != null)
                {
                    //defino que o endereço de entrega é igual ao capturado
                    pedido.EnderecoEntrega = endereco;
                    //coloco a dataentrega como agora  e salvo
                    pedido.DataEntrega = DateTime.Now;
                    
                    if (await _context.SaveChangesAsync() > 0)
                        TempData["mensagem"] = MensagemModel.Serializar("Entrega registrada com sucesso");
                    else
                        TempData["mensagem"] = MensagemModel.Serializar("Não foi possivel Registrar a entrega",
                        TipoMensagem.Erro);
                    //agora vou redirecionar para a ação Index passando a id cliente
                    return RedirectToAction("Index", new { cid = pedido.IdCliente });
                }
                else
                {
                    TempData["mensagem"] = MensagemModel.Serializar("Endereço não encontrado.",
                    TipoMensagem.Erro);
                    return RedirectToAction("Index", new { cid = pedido.IdCliente });
                }

            }
            else
            {
                TempData["mensagem"] = MensagemModel.Serializar("Pedido não encontrado",
                     TipoMensagem.Erro);
                return RedirectToAction("Index", "Cliente");
            }




        }


    }

}