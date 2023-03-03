using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class BaseEntity
    {
        //If you don't want to use EF core's auto-generating primary key values feature, you can turn it off.
        //You can add your data to the primary key It should resolve the error - Set Identity Insert off
        /// <summary>
        /// [DatabaseGenerated(DatabaseGeneratedOption.None)]
        /// </summary>
        public int Id { get; set; }
    }
}
