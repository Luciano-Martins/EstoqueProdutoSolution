using System.Linq;
using System.Threading.Tasks;
using EstoqueWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EstoqueWeb.Controllers
{
    public class ItemPedidoController : Controller
    {
        //crio uma propriedade privada, somente leitura, para guardar o contexto do banco de dados
        //que vem no parametro do construtor
        private readonly EstoqueWebContext _context;
        //aqui crio um construtor , vai receber context do banco
        public ItemPedidoController(EstoqueWebContext context)
        {
            this._context = context ;
            
        }
        public async Task<IActionResult> Index(int? ped)
        {
            //testamos ped para ver se tem um valor
           if (ped.HasValue)
           { 
            //pego o banco , na tabela pedido e vejo se tem um idPedido igual ao ped
             if (_context.Pedidos.Any(p => p.IdPedido == ped))
             {
                    // agora vou carregar esse pedido, através da função include vou inserir os dados
                    //do cliente e itens do pedido ondenando com a função OrderBy.
                    // usando a função ThenInclude , estou carregando os produtos de itensPedido,
                    //Especifica dados relacionados adicionais a serem incluídos com 
                    //base em um tipo relacionado que acabou de ser incluído
                 var pedido = await _context.Pedidos.Include(p => p.Cliente)
                .Include(p => p.ItensPedido.OrderBy(i => i.Produto.Nome))
                .ThenInclude(i => i.Produto)
                .FirstOrDefaultAsync(p => p.IdPedido == ped);
                //por fim eu filtro onde idPedido == ped e adiciono na ViewBag
                ViewBag.Pedido = pedido;
                return View(pedido.ItensPedido);
                
             }
             TempData["mensagem"] = MensagemModel.Serializar("Pedido Não encontrado",TipoMensagem.Erro);
             return RedirectToAction("Index", "Cliente"); 
           }
            TempData["mensagem"] = MensagemModel.Serializar("Pedido Não encontrado", TipoMensagem.Erro);
            return RedirectToAction("Index", "Cliente");


        }

        [HttpGet]
        public async Task<IActionResult> Cadastrar(int? ped ,int ? prod)
        
        {
            //já começo testando se idPedido tem um valor
            if (ped.HasValue)
            {
                //aqui já testo se tem algum idPedido == ped
                if (_context.Pedidos.Any(p => p.IdPedido ==ped))
                {
                    //eu busquei os produtos ordenados pelo nome 
                    //porem selecionei idProduto e estou criando um novo atributo que vai ser a concatenação
                    //do nome mais o preço que vai ficar entre parenteses logo a frete do nome
                    //trago como uma lista não rastreada
                    var produtos = _context.Produtos.OrderBy(x => x.Nome)
                    .Select(p => new { p.IdProduto, NomePreco = $"{p.Nome}({p.Preco:C})"}).AsNoTracking().ToList();
                    //aqui estou criando um objeto para uma lista de seleção usando a 
                    //coleção de produtos que acabei de pegar no banco de dados.
                    //sendo que o valor desta lista vai ser IdProduto e o que vai ser exibido vai ser NomePreco
                    //que é a concatenação nome preco
                    var produtosSelectList = new SelectList(produtos, "IdProduto","NomePreco");
                    //por fim guardo na ViewBag
                    ViewBag.Produtos= produtosSelectList;
                    //agora preciso testar se é uma alteração ou uma inclusão
                    if (prod.HasValue && ItemPedidoExiste(ped.Value, prod.Value))
                    {
                        // se o item existe eu capturo ele 
                        var itemPedido = await _context.ItensPedido
                        .Include(i => i.Produto)//incluo os dados do produto
                        .FirstOrDefaultAsync(i => i.IdPedido == ped && i.IdProduto == prod);
                        return View(itemPedido);// reorno a view passando este item de pedido
                    }
                    else
                    {
                        // caso não exista o idPedido e nem o idProduto
                        //retorno para a view um novo ItemPeido passando os valores
                        return View(new ItemPedidoModel()
                        {IdPedido = ped.Value , ValorUnitario = 0 , Quantidade = 1}
                        );
                    }
                    
                }
               
                    TempData["mensagem"] = MensagemModel.Serializar("Pedido Não Encontrado",TipoMensagem.Erro);
                    return RedirectToAction("Index", "Cliente");
            }
            TempData["mensagem"] = MensagemModel.Serializar("Pedido não Informado", TipoMensagem.Erro);
            return RedirectToAction("Index", "Cliente");
           
        }
    //     //vamos criar um métod para auxiliar , quando uma determinada itemPedido existe ou não, apartir
    //     //do id dela
        private bool ItemPedidoExiste(int ped , int prod )
        {
            return _context.ItensPedido.Any(x => x.IdPedido == ped &&
            x.IdProduto == prod);
        }
    //     //o parametro [fromform]indica que o itemPedidoModel esta vindo do formulario

        [HttpPost]//ele recebe um id de forma opcional,receber os dados do form no objeto
        //itemPedidoModel 
        public async Task<IActionResult>Cadastrar([FromForm] ItemPedidoModel itemPedido)
        {
            //na assinatura do método vou precisar do ItemPedidoModel completo
            //já testo se o stado é valido
            if (ModelState.IsValid)
            {
                //agora vou verificar se o idPedido é maior que zero
                if (itemPedido.IdPedido > 0)
                {
                    //vai ao banco e procuca um itemPedido com esse IdProduto
                    var produto = await _context.Produtos.FindAsync(itemPedido.IdProduto);
                    //aqui eu pego o preço do produto e atribuo no valor unitario do itemPedido
                    //independente de ser uma atualização ou inclusão eu sempre atualizo o preço
                    // com o valor atual
                    itemPedido.ValorUnitario = produto.Preco;
                    //agora preciso saber se este itemPedido existe, para sabe se é alteração ou exclusão
                    if (ItemPedidoExiste(itemPedido.IdPedido,itemPedido.IdProduto))
                    {
                        //se existe é alteração
                        _context.ItensPedido.Update(itemPedido);
                        //testo  se a alteração foi salva no banco
                        if (await _context.SaveChangesAsync() > 0)
                        {
                            // se sim , mando a mensagem de sucesso
                            TempData["mensagem"] = MensagemModel.Serializar("Item de Pedido Alterado com sucesso");
                            
                        }
                        else
                        {
                            //caso não , mensagem de erro
                            TempData["mensagem"] = MensagemModel.Serializar("Erro ao alterar item de pedido",
                            TipoMensagem.Erro);
                        }  
                    }
                    else
                    {
                        //se entrar aqui é inclusão
                        _context.ItensPedido.Add(itemPedido);
                        if (await _context.SaveChangesAsync() > 0)
                        {
                            // se sim , mando a mensagem de sucesso
                            TempData["mensagem"] = MensagemModel.Serializar("Item de Pedido cadastrado sucesso");
                        }
                        else
                        {
                            //caso não , mensagem de erro
                            TempData["mensagem"] = MensagemModel.Serializar("Erro ao cadastrar item de pedido",
                            TipoMensagem.Erro);
                        }
                    }
                    //agora preciso atualizar o valor total
                    //agora vou ao banco , pego o id do item de pedido
                    var pedido = await _context.Pedidos.FindAsync(itemPedido.IdPedido);
                    //agora pedido.valortotal onde id é igual id e valorUnitario  8 quantidade e salva
                    pedido.ValorTotal = _context.ItensPedido
                    .Where(i => i.IdPedido == itemPedido.IdPedido)
                    .Sum(i => i.ValorUnitario * i.Quantidade);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", new { ped = itemPedido.IdPedido });
                }
                else
                {
                    TempData["mensagem"] = MensagemModel.Serializar("Pedido Não informado",TipoMensagem.Erro);
                    return RedirectToAction("Index", "Cliente");
                }     
            }
            else
            {
                //aqui estou criando um objeto para uma lista de seleção usando a 
                //coleção de produtos que acabei de pegar no banco de dados.
                //sendo que o valor desta lista vai ser IdProduto e o que vai ser exibido vai ser NomePreco
                //que é a concatenação nome preco
                var produtos = _context.Produtos
                .OrderBy(x => x.Nome)
                .Select(p => new { p.IdProduto, NomePreco = $"{p.Nome}({p.Preco:C})" })
                .AsNoTracking().ToList();
                var produtosSelectList = new SelectList(produtos, "IdProduto", "NomePreco");
                //por fim guardo na ViewBag
                ViewBag.Produtos = produtosSelectList;
                //retorno para  Index, passando o id do pedido , 
                //atribuo ao ped para ele ser intepretado la na view , mostrando só os
                //itens desse pedido
                //esse else caso o ModelState não seja valido
                //retorno para View do formulario passando novamente o itemPedido
                return View(itemPedido);
            }  
        }
       
       [HttpGet]
       public async Task<IActionResult>Excluir(int? ped , int? prod)
       {
         if (!ped.HasValue || !prod.HasValue)
         {
            TempData["mensagem"] = MensagemModel.Serializar("Item Pedido Não Informado.",
            TipoMensagem.Erro);
            return RedirectToAction("Index", "Cliente");
         }
         if (!ItemPedidoExiste(ped.Value,prod.Value))
         {
            TempData["mensagem"] = MensagemModel.Serializar("Item de pedido não encontrado",
            TipoMensagem.Erro);
            return RedirectToAction("Index", "Cliente");
         }
            var itemPedido = await _context.ItensPedido.FindAsync(ped,prod);
            //quando vou carregar um unico objeto relacionado , eu uso reference
            _context.Entry(itemPedido).Reference(i => i.Produto).Load();
            // isso é o suficiente para exibir o formulario de exclusão
            return View(itemPedido);
       }


       [HttpPost]
       public async Task<IActionResult>Excluir(int idPedido, int idProduto)
       {
         var itemPedido = await _context.ItensPedido.FindAsync(idPedido,idProduto);

         if (itemPedido != null)
         {
            _context.ItensPedido.Remove(itemPedido);
            if (await _context.SaveChangesAsync() > 0)
             {
                    TempData["mensagem"] = MensagemModel.Serializar("Item de Pedido Excluido com sucesso");
                    var pedido = await _context.Pedidos.FindAsync(itemPedido.IdPedido);
                    pedido.ValorTotal = _context.ItensPedido
                    .Where(i => i.IdPedido == itemPedido.IdPedido)
                    .Sum(i => i.ValorUnitario * i.Quantidade);
                    await _context.SaveChangesAsync();
             }
            else
            
                    TempData["mensagem"] = MensagemModel.Serializar("Não foi possivel excluir a item do Pedido",
                       TipoMensagem.Erro);       
                    return RedirectToAction("Index", new { ped = idPedido });        
         }
         else
         {
                TempData["mensagem"] = MensagemModel.Serializar("Item de Pedido não encontrado",
                    TipoMensagem.Erro);
                return RedirectToAction("Index", new { ped = idPedido });
         }  
        } 
    }
}