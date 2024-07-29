using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Loyufei.DomainEvents;

namespace MineSweeper
{
    public class Presenter : Loyufei.MVP.Presenter
    {
        public Presenter(DomainEventService service) : base(service)
        {

        }

        public override object GroupId => Declarations.MineSweeper;
    }
}
