using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Its.Kaspa.Filter.Models;

[Table("Blocks")]
public class Block
{
    [Key]
    public Guid? Id { get; set; }
    public string BlockHash { get; set; }
    public string SelectedParentHash { get; set; }

    public Block()
    {
        BlockHash = "";
        SelectedParentHash = "";
    }
}
