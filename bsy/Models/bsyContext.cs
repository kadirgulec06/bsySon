using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Collections;
using Npgsql;

namespace bsy.Models

{
    /*
    public class AuditTrailFactory
    {
        private readonly DbContext context;        
        public AuditTrailFactory(DbContext context)
        {
            this.context = context;
        }

        public Audit GetAudit(DbEntityEntry entry)
        {
            var audit = new Audit();
            audit.UserId = HttpContext.Current != null ? HttpContext.Current.User.Identity.Name : "";
            //Change this line according to your needs
            audit.TableName = GetTableName(entry);
            if (audit.TableName.Contains("_"))
                audit.TableName = audit.TableName.Split('_')[0];
            audit.UpdateDate = DateTime.Now;
            audit.TableIdValue = GetKeyValue(entry);

            //entry is Added 
            if (entry.State == EntityState.Added)
            {
                var newValues = new StringBuilder();
                SetAddedProperties(entry, newValues);
                audit.NewData = newValues.ToString();
                audit.Actions = AuditActions.I.ToString();
            }
            //entry in deleted
            else if (entry.State == EntityState.Deleted)
            {
                var oldValues = new StringBuilder();
                SetDeletedProperties(entry, oldValues);
                audit.OldData = oldValues.ToString();
                audit.Actions = AuditActions.D.ToString();
            }
            //entry is modified
            else if (entry.State == EntityState.Modified)
            {
                var oldValues = new StringBuilder();
                var newValues = new StringBuilder();
                SetModifiedProperties(entry, oldValues, newValues);
                audit.OldData = oldValues.ToString();
                audit.NewData = newValues.ToString();
                audit.Actions = AuditActions.U.ToString();
            }

            return audit;
        }

        private void SetAddedProperties(DbEntityEntry entry, StringBuilder newData)
        {
            foreach (var propertyName in entry.CurrentValues.PropertyNames)
            {
                var newVal = entry.CurrentValues[propertyName];
                if (newVal != null)
                {
                    newData.AppendFormat("{0}={1} || ", propertyName,
                        newVal is byte[] ? ((byte[])newVal).ToString() : newVal);
                }
            }
            if (newData.Length > 0)
                newData = newData.Remove(newData.Length - 3, 3);
        }

        private void SetDeletedProperties(DbEntityEntry entry, StringBuilder oldData)
        {
            var dbValues = entry.GetDatabaseValues();
            foreach (var propertyName in dbValues.PropertyNames)
            {
                var oldVal = dbValues[propertyName];
                if (oldVal != null)
                {
                    oldData.AppendFormat("{0}={1} || ", propertyName,
                        oldVal is byte[] ? ((byte[])oldVal).ToString() : oldVal);
                }
            }
            if (oldData.Length > 0)
                oldData = oldData.Remove(oldData.Length - 3, 3);
        }

        private void SetModifiedProperties(DbEntityEntry entry, StringBuilder oldData, StringBuilder newData)
        {
            var dbValues = entry.GetDatabaseValues();
            foreach (var propertyName in entry.OriginalValues.PropertyNames)
            {
                var oldVal = dbValues[propertyName];
                var newVal = entry.CurrentValues[propertyName];
                if (!Equals(oldVal, newVal))
                {
                    newData.AppendFormat("{0}={1} || ", propertyName, newVal);
                    oldData.AppendFormat("{0}={1} || ", propertyName, oldVal);
                }
            }
            if (oldData.Length > 0)
                oldData = oldData.Remove(oldData.Length - 3, 3);
            if (newData.Length > 0)
                newData = newData.Remove(newData.Length - 3, 3);
        }

        public long? GetKeyValue(DbEntityEntry entry)
        {
            long id = 0;
            try
            {
                var objectStateEntry =
                    ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.GetObjectStateEntry(entry.Entity);
                if (objectStateEntry.EntityKey.EntityKeyValues != null)
                    id = Convert.ToInt64(objectStateEntry.EntityKey.EntityKeyValues[0].Value);
            }
            catch (Exception ex)
            {
                return -1;
            }
            return id;
        }

        private string GetTableName(DbEntityEntry dbEntry)
        {
            var tableAttr =
                dbEntry.Entity.GetType().GetCustomAttributes(typeof(TableAttribute), false).SingleOrDefault() as
                    TableAttribute;
            var tableName = tableAttr != null ? tableAttr.Name : dbEntry.Entity.GetType().Name;
            return tableName;
        }
    }

    public enum AuditActions
    {
        I,
        U,
        D
    }
*/

    using Npgsql;
    class NpgSqlConfiguration : DbConfiguration
    {
        public NpgSqlConfiguration()
        {
            var name = "Npgsql";

            SetProviderFactory(providerInvariantName: name,
                               providerFactory: NpgsqlFactory.Instance);

            SetProviderServices(providerInvariantName: name,
                                provider: NpgsqlServices.Instance);

            SetDefaultConnectionFactory(connectionFactory: new NpgsqlConnectionFactory());
        }
    }

    public class bsyContext : DbContext
    {
        private readonly IList auditList = new List<object>();
        private readonly IList list = new List<object>();
        //private AuditTrailFactory auditFactory;
        private string[] dahilOlmayanlar = new string[] { "LogModel" };
        public bsyContext()
            : base("name=BSYContext")
        {
            Database.SetInitializer<bsyContext>(null);
        }

        //public DbSet<MusteriSirket> MusteriSirket { get; set; }
        //public DbSet<DegerlemeUzmanlari> DegerlemeUzmanlari { get; set; }

        public DbSet<DENEME> tblDeneme { get; set; }
        public DbSet<BSYMENUSU> tblBSYmenusu { get; set; }
        public DbSet<SOZLUK> tblSozluk { get; set; }
        public DbSet<KULLANICI> tblKullanicilar { get; set; }
        public DbSet<ROLLER> tblRoller { get; set; }
        public DbSet<GIRISDENEME> tblGirisDenemeleri { get; set; }
        public DbSet<SIFREDEGISME> tblSifreDegisme { get; set; }
        public DbSet<SIFRESIFIRLA> tblSifreSifirla { get; set; }
        public DbSet<EPOSTAACMA> tblEPostaAcma { get; set; }
        public DbSet<IPACMA> tblIPAcma { get; set; }
        public DbSet<KULLANICIROL> tblKullaniciRolleri { get; set; }
        public DbSet<BOLGE> tblBolgeler { get; set; }
        public DbSet<SEHIR> tblSehirler { get; set; }
        public DbSet<ILCE> tblIlceler { get; set; }
        public DbSet<MAHALLE> tblMahalleler { get; set; }
        public DbSet<GOREVSAHASI> tblGorevSahasi { get; set; }

        /*
        public override int SaveChanges()
        {
            auditList.Clear();
            list.Clear();
            auditFactory = new AuditTrailFactory(this);
            var entityList =
                ChangeTracker.Entries()
                    .Where(
                        p =>
                            p.State == EntityState.Added || p.State == EntityState.Deleted ||
                            p.State == EntityState.Modified);
            foreach (var entity in entityList)
            {
                var audit = auditFactory.GetAudit(entity);
                if (!dahilOlmayanlar.Contains(audit.TableName))
                {
                    auditList.Add(audit);
                    list.Add(entity);
                }
                //DbEntityValidationResult vr0 = this.Entry(entity).Entity.GetValidationResult();
            }

            var retVal = 0;
            try
            {
                retVal = base.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Debug.Write(string.Format("Entity türü \"{0}\" şu hatalara sahip \"{1}\" Geçerlilik hataları:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Debug.Write(string.Format("- Özellik: \"{0}\", Hata: \"{1}\"", ve.PropertyName, ve.ErrorMessage));
                    }
                }
            }
            catch (Exception ex)
            {
                string mesaj = DebugHelper.exceptionMesaji(ex);
                Debug.Write(mesaj);
            }

            if (auditList.Count > 0)
            {
                var i = 0;
                foreach (Audit audit in auditList)
                {
                    if (audit.Actions == AuditActions.I.ToString())
                        audit.TableIdValue = auditFactory.GetKeyValue(list[i] as DbEntityEntry);
                    Audits.Add(audit);
                    i++;
                }
                base.SaveChanges();
            }

            return retVal;
        }

        */

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new DenemeConfiguration());
            modelBuilder.Configurations.Add(new bsyMenusuConfiguration());
            modelBuilder.Configurations.Add(new SozlukConfiguration());
            modelBuilder.Configurations.Add(new KullaniciConfiguration());
            modelBuilder.Configurations.Add(new RollerConfiguration());
            modelBuilder.Configurations.Add(new EPostaAcmaConfiguration());
            modelBuilder.Configurations.Add(new IPAcmaConfiguration());
            modelBuilder.Configurations.Add(new GirisDenemeleriConfiguration());
            modelBuilder.Configurations.Add(new SifreDegismeConfiguration());
            modelBuilder.Configurations.Add(new SifreSifirlaConfiguration());
            modelBuilder.Configurations.Add(new KullaniciRolleriConfiguration());
            modelBuilder.Configurations.Add(new BolgelerConfiguration());
            modelBuilder.Configurations.Add(new SehirlerConfiguration());
            modelBuilder.Configurations.Add(new IlcelerConfiguration());
            modelBuilder.Configurations.Add(new MahallelerConfiguration());
            modelBuilder.Configurations.Add(new GorevSahasiConfiguration());

            base.OnModelCreating(modelBuilder);
        }

        public class DenemeConfiguration : EntityTypeConfiguration<DENEME>
        {
            public DenemeConfiguration()
                : base()
            {
                HasKey(p => p.id);
                Property(p => p.id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                ToTable("deneme");
            }
        }

        public class bsyMenusuConfiguration : EntityTypeConfiguration<BSYMENUSU>
        {
            public bsyMenusuConfiguration()
                : base()
            {
                HasKey(p => p.menuNo);
                ToTable("bsyMenusu");
            }
        }

        public class SozlukConfiguration : EntityTypeConfiguration<SOZLUK>
        {
            public SozlukConfiguration()
                : base()
            {
                HasKey(p => p.id);
                Property(p => p.id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                ToTable("Sozluk");
            }
        }

        public class KullaniciConfiguration : EntityTypeConfiguration<KULLANICI>
        {
            public KullaniciConfiguration()
                : base()
            {
                HasKey(p => p.id);
                Property(p => p.id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                ToTable("Kullanicilar");
            }
        }

        public class RollerConfiguration : EntityTypeConfiguration<ROLLER>
        {
            public RollerConfiguration()
                : base()
            {
                HasKey(p => p.id);
                Property(p => p.id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                ToTable("Roller");
            }
        }

        public class EPostaAcmaConfiguration : EntityTypeConfiguration<EPOSTAACMA>
        {
            public EPostaAcmaConfiguration()
                : base()
            {
                HasKey(p => p.id);
                Property(p => p.id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                ToTable("EPostaAcma");
            }
        }

        public class IPAcmaConfiguration : EntityTypeConfiguration<IPACMA>
        {
            public IPAcmaConfiguration()
                : base()
            {
                HasKey(p => p.id);
                Property(p => p.id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                ToTable("IPAcma");
            }
        }

        public class GirisDenemeleriConfiguration : EntityTypeConfiguration<GIRISDENEME>
        {
            public GirisDenemeleriConfiguration()
                : base()
            {
                HasKey(p => p.id);
                Property(p => p.id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                ToTable("GirisDenemeleri");
            }
        }
        public class SifreDegismeConfiguration : EntityTypeConfiguration<SIFREDEGISME>
        {
            public SifreDegismeConfiguration()
                : base()
            {
                HasKey(p => p.id);
                Property(p => p.id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                ToTable("SifreDegisme");
            }
        }

        public class SifreSifirlaConfiguration : EntityTypeConfiguration<SIFRESIFIRLA>
        {
            public SifreSifirlaConfiguration()
                : base()
            {
                HasKey(p => p.id);
                Property(p => p.id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                ToTable("SifreSifirla");
            }
        }

        public class KullaniciRolleriConfiguration : EntityTypeConfiguration<KULLANICIROL>
        {
            public KullaniciRolleriConfiguration()
                : base()
            {
                HasKey(p => p.id);
                Property(p => p.id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                ToTable("KullaniciRolleri");
            }
        }

        public class BolgelerConfiguration : EntityTypeConfiguration<BOLGE>
        {
            public BolgelerConfiguration()
                : base()
            {
                HasKey(p => p.id);
                ToTable("Bolgeler");
            }
        }

        public class SehirlerConfiguration : EntityTypeConfiguration<SEHIR>
        {
            public SehirlerConfiguration()
                : base()
            {
                HasKey(p => p.id);
                ToTable("Sehirler");
            }
        }

        public class IlcelerConfiguration : EntityTypeConfiguration<ILCE>
        {
            public IlcelerConfiguration()
                : base()
            {
                HasKey(p => p.id);
                ToTable("Ilceler");
            }
        }

        public class MahallelerConfiguration : EntityTypeConfiguration<MAHALLE>
        {
            public MahallelerConfiguration()
                : base()
            {
                HasKey(p => p.id);
                ToTable("Mahalleler");
            }
        }

        public class GorevSahasiConfiguration : EntityTypeConfiguration<GOREVSAHASI>
        {
            public GorevSahasiConfiguration()
                : base()
            {
                HasKey(p => p.id);
                Property(p => p.id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                ToTable("GorevSahasi");
            }
        }

    }
}
