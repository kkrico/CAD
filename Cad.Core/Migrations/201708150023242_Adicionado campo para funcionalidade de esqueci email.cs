namespace Cad.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Adicionadocampoparafuncionalidadedeesqueciemail : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.USUARIO", "HasAlteracaoSenha", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.USUARIO", "HasAlteracaoSenha");
        }
    }
}
