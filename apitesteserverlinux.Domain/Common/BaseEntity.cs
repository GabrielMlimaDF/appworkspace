namespace apitesteserverlinux.Domain;

public abstract class BaseEntity
{
   
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public Guid RequestUserId { get; private set; }

    protected BaseEntity()
    {
        CreatedAt = DateTime.UtcNow;
    } 
    public void SetCreatedBy(Guid userId)
    {
        RequestUserId = userId;
    }
}
