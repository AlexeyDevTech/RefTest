using ANG24.Core.Controllers.ReflectController;
using ANG24.Core.Interfaces;
using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANG24.Core
{
    public sealed class ConfiguratorCore : Module
    {

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ReflectController120>().Named<IReflectController>("Reflect120").SingleInstance();
            builder.RegisterType<ReflectController90>().Named<IReflectController>("Reflect90").SingleInstance();
            builder.RegisterType<ReflectService>().As<IReflectService>().SingleInstance();
            builder.RegisterType<ReflectSmoothService>().As<IReflectSmoothService>().SingleInstance();
        }
    }
}
