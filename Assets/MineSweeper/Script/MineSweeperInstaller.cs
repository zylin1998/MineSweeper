using UnityEngine;
using Zenject;
using Loyufei;
using Loyufei.DomainEvents;

namespace MineSweeper
{
    public class MineSweeperInstaller : MonoInstaller
    {
        [SerializeField]
        private MineSprites _MineSprites;
        [SerializeField]
        private GameObject  _MineListener;

        public override void InstallBindings()
        {
            #region Factory

            Container
                .BindMemoryPool<MineListener, MineListener.Pool>()
                .WithInitialSize(500)
                .FromComponentInNewPrefab(_MineListener)
                .AsCached();

            #endregion

            #region Data Structure

            Container.
                Bind<MineSprites>()
                .FromInstance(_MineSprites)
                .AsSingle();

            Container
                .Bind<MineSweeperGrid>()
                .AsSingle();

            Container
                .Bind<MineSweeperQuery>()
                .AsSingle();

            Container
                .Bind<Loyufei.Random>()
                .AsSingle();

            #endregion

            #region Model

            Container
                .Bind<MineSweeperModel>()
                .AsSingle();

            Container
                .Bind<DataUpdater>()
                .AsSingle();

            Container
                .Bind<Timer>()
                .AsSingle();

            #endregion

            #region Presenter

            Container
                .Bind<MineSweeperPresenter>()
                .AsSingle()
                .NonLazy();

            Container
                .Bind<MineSweeperViewPresenter>()
                .AsSingle()
                .NonLazy();

            Container
                .Bind<SettingViewPresenter>()
                .AsSingle()
                .NonLazy();

            #endregion

            #region Event

            SignalBusInstaller.Install(Container);

            Container
                .DeclareSignal<IDomainEvent>()
                .WithId(Declarations.MineSweeper);

            Container
                .Bind<IDomainEventBus>()
                .To<DomainEventBus>()
                .AsCached()
                .WithArguments(Declarations.MineSweeper);

            Container
                .Bind<DomainEventService>()
                .AsSingle();

            #endregion
        }
    }
}