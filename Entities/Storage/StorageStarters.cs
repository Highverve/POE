using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Pilgrimage_Of_Embers.Entities.Storage
{
    public class EmptyStorage : EntityStorage { public EmptyStorage() : base(new List<Point>() { }) { } }
    public class BasicsStorage : EntityStorage { public BasicsStorage() : base(new List<Point>() { new Point(2, 5) }) { } }

}
