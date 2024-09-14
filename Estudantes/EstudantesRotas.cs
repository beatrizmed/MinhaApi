using Microsoft.EntityFrameworkCore;
using MinhaApi.Data;

namespace MinhaApi.Estudantes;

public static class EstudantesRotas
{
    public static void AddRotasEstudantes(this WebApplication app)
    {
        var rotasEstudantes = app.MapGroup("estudantes");
        
        //para criar se o Post
        rotasEstudantes.MapPost("", 
            handler:async (AddEstudanteRequest request, AppDbContext context, CancellationToken ct) =>
        {
            var jaExiste = await context.Estudantes
                .AnyAsync(estudante => estudante.Nome == request.Nome, ct);
           
            if(jaExiste)
                return Results.Conflict(error: "Estudante já existe!");
            
            var novoEstudante = new Estudante(request.Nome);
            await context.Estudantes.AddAsync(novoEstudante, ct); 
            await context.SaveChangesAsync(ct);
            
            var estudanteRetorno = new EstudanteDto(novoEstudante.Id, novoEstudante.Nome);
            return Results.Ok(estudanteRetorno);
        });
        
        //retornar todos os estudantes cadastrados
        rotasEstudantes.MapGet("", handler:async (AppDbContext context, CancellationToken ct) =>
        {
           var estudantes = await context
               .Estudantes
               .Where(estudante => estudante.Ativo)
               .Select(estudante => new EstudanteDto(estudante.Id, estudante.Nome))
               .ToListAsync(ct);
           
           return estudantes;
        });
        
        //atualizar/alterar nome dos estudantes cadastrados
        rotasEstudantes.MapPut("{id:guid}",
            handler:async (Guid id ,UpdateEstudanteRequest request, AppDbContext context, CancellationToken ct) =>
        {
            var estudante = await context.Estudantes
                .SingleOrDefaultAsync(predicate:estudante => estudante.Id == id, ct);
            
            if(estudante == null)
                return Results.NotFound();
            
            estudante.AtualizarNome(request.Nome);
            
            await context.SaveChangesAsync(ct);
            return Results.Ok(new EstudanteDto(estudante.Id, estudante.Nome));
        });
        
        //deletar 
        rotasEstudantes.MapDelete("{id}",
                async (Guid id, AppDbContext context, CancellationToken ct) =>
        {
            var estudante = await context.Estudantes
                .SingleOrDefaultAsync(estudante => estudante.Id == id, ct);
            
            if(estudante == null)
                return Results.NotFound();
            
            estudante.Desativar();
            
            await context.SaveChangesAsync(ct);
            return Results.Ok();
        });

    }
}