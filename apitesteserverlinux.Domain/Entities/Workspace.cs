namespace apitesteserverlinux.Domain.Entities;


    public class Workspace
    {
        public Guid Id { get; private set; } = Guid.NewGuid();

        public string Name { get; private set; } = string.Empty;

        // Dono do workspace (único Admin do workspace)
        public Guid OwnerUserId { get; private set; }

        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        private Workspace() { } // EF Core

        public Workspace(string name, Guid ownerUserId)
        {
            Name = name.Trim();
            OwnerUserId = ownerUserId;
            CreatedAt = DateTime.UtcNow;
        }

        public void Rename(string name)
        {
            Name = name.Trim();
        }
    }




