using System;
using System.Linq;
using System.Threading.Tasks;
using EstoqueWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EstoqueWeb.Controllers
{
    public class EnderecoController : Controller
    {
        //crio uma propriedade privada, somente leitura, para guardar o contexto do banco de dados
        //que vem no parametro do construtor
        private readonly EstoqueWebContext _context;
        //aqui crio um construtor , vai receber context do banco
        public EnderecoController(EstoqueWebContext context)
        {
            this._context = context ;
            
        }
        public async Task<IActionResult> Index(int? cid)
        {
            if (cid.HasValue)//testo para ver se o cliente tem um valor
            {  
                //agora tento carregar o conteudo na variavel cliente
                var cliente = await _context.Clientes.FindAsync(cid);
                //agora eu testo para ver se conseguiu
                if (cliente != null)
                {
                    // se sim a coleção desse cliente será carregada
                    _context.Entry(cliente).Collection(c => c.Enderecos).Load();
                    ViewBag.Cliente = cliente;
                    //retorna para a View e mostra o endereço do cliente especifico
                    return View(cliente.Enderecos);   
                }
                else
                {
                    TempData["mensagem"] = MensagemModel.Serializar("Cliente não foi encontrado", TipoMensagem.Erro);
                    return RedirectToAction("Index", "Cliente");
                }     
            }
            else
            {
                TempData["mensagem"] = MensagemModel.Serializar("Só é possivel mostrar endereços de um cliente especifico .",TipoMensagem.Erro);
                return RedirectToAction("Index", "Cliente");
            }  
        }
         [HttpGet]//essa Action vai responder apenas requisições do tipo Get
        public async Task<IActionResult> Cadastrar(int? cid , int? eid) 
        {
            //primeiro verifico se o id de cliente foi passado, se não , passo a mensagem no else
            if (cid.HasValue)
            {
                //caso foi passado o id , tento capturar essa id e coloco na variavel cliente
                var cliente = await _context.Clientes.FindAsync(cid);
               //testo para ver se é diferente de null
                if (cliente != null)
                {
                    //se for, eu armazeno na ViewBag
                    ViewBag.Cliente = cliente;
                    //agora eu verifico se o id de endereco tem um valor
                    //senão tiver valor , significa que é uma inclusão
                    //retorno na View uma new EnderecoModel

                    if (eid.HasValue)
                    {
                        //se tiver um valor eu carrego os endereços do cliente
                        _context.Entry(cliente).Collection(c => c.Enderecos).Load();
                        //capturo o objeto endereço, referente a esse idEndereco
                        var endereco = cliente.Enderecos.FirstOrDefault(e => e.IdEndereco == eid);
                        //testo para ver se esse objeto é diferente de null
                        if (endereco != null)
                        {
                            // se sim passo ele para view para ser alterado
                            return View(endereco);    
                        }
                        else
                        {
                            //caso seja null , deixo essa mensagem
                            TempData["mensagem"] =  MensagemModel.Serializar("Endereço não encontrado.", 
                            TipoMensagem.Erro);
                        }
                    }
                    else
                    {
                        return View(new EnderecoModel());
                    }    
                }
                else
                {
                    TempData["mensagem"] = MensagemModel.Serializar("Cliente Não Encontrado",TipoMensagem.Erro);
                }
                return RedirectToAction("Index");
            }
            else
            {
                TempData["mensagem"] = MensagemModel.Serializar(
                    "Nenhum proprietario de endereços foi informado ."
                , TipoMensagem.Erro);
                //senão encontrado id do cliente, retorno para index de cliente,
                // ou seja volta a listagem de clientes
                return RedirectToAction("Index","Cliente");
            }
        }
        //vamos criar um métod para auxiliar , quando uma determinada endereco existe ou não, apartir
        //do id dela
        private bool EnderecoExiste( int cid ,int eid )
        {
            return _context.Clientes.FirstOrDefault(c => c.IdUsuario == cid).Enderecos.Any(e => e.IdEndereco == eid);
            
        }
        //o parametro [fromform] indica que o enderecoModel esta vindo do formulario
        [HttpPost]//junto o idUsuario
        public async Task<IActionResult>Cadastrar([FromForm] int? idUsuario,

             [FromForm]EnderecoModel endereco)
        {
            //testar se o idUsuario tem um valor
           if (idUsuario.HasValue)
           {
           //se sim vamos capiturar o valor e atribuir a variavel
             var cliente = await _context.Clientes.FindAsync(idUsuario);
             //agora coloco na ViewBag
             ViewBag.Cliente = cliente;
             //vamos testar se o modelState é valido, ou seja, se o objeto endereço esta,
             //comprindo todos os requisitos de validação definido na classe.
             //caso o bjeto endereco seja valido.
             if(ModelState.IsValid)
             {
                //outro teste para saber se o cliente ja possui um endereco, se não tem , tenho que fazer 
                // o endereço que esta vindo seja o endereco padrão.
                if (cliente.Enderecos.Count()== 0) endereco.Selecionado = true;
                endereco.CEP = ObterCepNormalizado(endereco.CEP);
                //verificar id endereco é maior que 0 , se for significa que é uma alteração
                if (endereco.IdEndereco > 0 )
                {
                    // se este endereco esta Selecionado , significa que quero tornalo padrão, 
                    //então tenho que desmarcar os demais
                    if(endereco.Selecionado)
                    //para cada endereço do cliente vou marcar a propriedade Selecionado como false.
                       cliente.Enderecos.ToList().ForEach(e => e.Selecionado = false);
                       //agora se o endereço existe , passando o valor de idUsuario e endereco.idEndereco
                    if (EnderecoExiste(idUsuario.Value, endereco.IdEndereco))
                    {
                            // vou capturar o endereço existente, que é o que vai ser alterado
                            // FirstOrDefault =>  Retorna o primeiro elemento de uma sequência 
                            //ou um valor padrão caso não seja encontrado nenhum elemento.
                            var enderecoAtual = cliente.Enderecos.FirstOrDefault(e => 
                        e.IdEndereco == endereco.IdEndereco);
                            //estamos pegando os dados do endereco que vem do formulario e alimentando o endereço
                            // atual, ou seja, estou atualizando os dados do banco, com os dados que veio do formulario
                            // Entry => A entrada fornece acesso a informações e operações de controle de alterações para a entidade.
                            _context.Entry(enderecoAtual).CurrentValues.SetValues(endereco);
                        //agora vou testar se o endereco esta, não modificado, significa que o setvalues não deu nehum resultado
                        //ou seja esta com valores iguais
                        if(_context.Entry(enderecoAtual).State == EntityState.Unchanged)
                        {
                            TempData["mensagem"] = MensagemModel.Serializar("Nenhum dado do endereço foi alterado");
                        }
                        else
                        {
                            //caso algum dado  foi alterado, vamos tentar salvar
                            if (await _context.SaveChangesAsync() > 0)
                            {
                                TempData["mensagem"] = MensagemModel.Serializar("Endereço alterado com sucesso.");
                            }else
                            {
                                TempData["mensagem"] = MensagemModel.Serializar("Erro ao Alterar o endereço");
                            }
                        }
                    }
                    else
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("Endereço não encontrado",TipoMensagem.Erro);
                    }     
                }
                else
                {
                    //aqui quer dizer que é uma inclusão de endereço
                    // endereço de cliente > 0 e pego o valor de endereço maior que existe na coleção endereço e somo 1
                    //caso contrario o id vai ser 1
                    var idEndereco = cliente.Enderecos.Count() > 0 ? cliente.Enderecos.Max(e => e.IdEndereco) + 1 : 1;
                    // agora vou fazer o objeto endereço , receber esse id endereço
                    endereco.IdEndereco = idEndereco;
                    //aqui eu seleciono o cliente dono desse endereço, pego a coleção de endereço dele , e adiciono
                    //esse novo objeto endereço nele, que acobo de prencher o id que o restos dos dados vem do formulario
                    _context.Clientes.FirstOrDefault(c => c.IdUsuario == idUsuario).Enderecos.Add(endereco);
                    //agora vamos salvar
                    if (await _context.SaveChangesAsync() > 0)
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("Endereço cadastrado com sucesso");
                    }
                    else
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("Erro ao cadastrar endereço.",TipoMensagem.Erro);
                    }
                }
                // no retorno da ação,passo a Index da controler Endereco, passando na rota o parametro cid == idUsuario 
                return RedirectToAction("Index", "Endereco", new {cid = idUsuario});
            }
            else
            {
                return View(endereco);
            }
          }
          else
          {
            TempData["mensagem"] = MensagemModel.Serializar("Nunhum proprietario de endereços foi informado",TipoMensagem.Erro);
            return RedirectToAction("Index" , "Cliente");
          }
        }
        private string ObterCepNormalizado(string cep)
        {
            //recebo o cep com a função Replace eu troco - por vazio e . por vazio
            //e retiro espaços laterais com a função Trim
            string cepNormalizado = cep.Replace("-","").Replace(".","").Trim();
            //aqui uso a função Insert para dizer que após a posição 5 adicionar caractere de ifem (-)
            return cepNormalizado.Insert(5 , "-");  
        }



           [HttpGet]
           public async Task<IActionResult>Excluir(int? cid, int? eid)
           {
            //vamos testar se tem um idUsuario e um idEndereco
              if (!cid.HasValue)
              {
                TempData["mensagem"] = MensagemModel.Serializar("Cliente não informado",TipoMensagem.Erro);
                return RedirectToAction("Index", "Cliente");
              }
            if (!eid.HasValue)
            {
                TempData["mensagem"] = MensagemModel.Serializar("Endereço não informado", TipoMensagem.Erro);
                return RedirectToAction("Index", new { cid = cid });
            }
            //caso passe pelas validações, vou capturar o cliente
            var cliente = await _context.Clientes.FindAsync(cid);
            var endereco = cliente.Enderecos.FirstOrDefault(e => e.IdEndereco == eid);
            if (endereco == null)
            {
                TempData["mensagem"] = MensagemModel.Serializar("Endeço não Encontrado");
                return RedirectToAction("Index", new {cid = cid });
            }
            ViewBag.Cliente = cliente;
            return View(endereco);
            
           }
           [HttpPost]
           public async Task<IActionResult>Excluir(int idUsuario , int idEndereco)
           {
             var cliente = await _context.Clientes.FindAsync(idUsuario);
             var endereco = cliente.Enderecos.FirstOrDefault(e => e.IdEndereco == idEndereco);
             if (endereco != null)
             {
                cliente.Enderecos.Remove(endereco);
                if (await _context.SaveChangesAsync() > 0)

                    TempData["mensagem"] = MensagemModel.Serializar("Endereço Excluido com sucesso !!!");
                    //se conseguir excluir e o usuário tiver mais endereços, ele transforma em padrão
                    if (endereco.Selecionado && cliente.Enderecos.Count() > 0)
                    {
                        cliente.Enderecos.FirstOrDefault().Selecionado = true;
                    }
                else
                    TempData["mensagem"] = MensagemModel.Serializar("Não foi possivel excluir a endereço",
                    TipoMensagem.Erro);  
             }
             else
             {
                    TempData["mensagem"] = MensagemModel.Serializar("Endereço não encontrado",
                        TipoMensagem.Erro);          
             }
            return RedirectToAction("Index", new {cid = idUsuario});


            }

        
    }
}