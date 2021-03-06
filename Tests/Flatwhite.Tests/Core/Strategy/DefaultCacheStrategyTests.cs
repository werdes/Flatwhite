﻿using System;
using System.Collections.Generic;
using Flatwhite.Provider;
using Flatwhite.Strategy;
using NSubstitute;
using NUnit.Framework;

namespace Flatwhite.Tests.Core.Strategy
{
    [TestFixture]
    public class DefaultCacheStrategyTests
    {
        [SetUp]
        public void SetUp()
        {
            Global.Init();
        }

        [Test]
        public void GetCacheStore_should_return_default_cache_store_if_id_not_found()
        {
            var invocation = Substitute.For<_IInvocation>();
            var context = new Dictionary<string, object>
            {
                [Global.__flatwhite_outputcache_attribute] = new OutputCacheAttribute
                {
                    CacheStoreId = 100
                }
            };
            var cacheStoreProvider = Substitute.For<ICacheStoreProvider>();
            cacheStoreProvider.When(x => x.GetCacheStore(100)).Do(x => { throw new KeyNotFoundException(); });
            Global.CacheStoreProvider = cacheStoreProvider;

            var stg = new DefaultCacheStrategy();

            // Action
            var store = stg.GetCacheStore(invocation, context);

            // Assert
            cacheStoreProvider.Received(1).GetCacheStore(0);
        }

        [Test]
        public void GetCacheStore_should_try_to_get_cache_store_by_provided_type()
        {
            var invocation = Substitute.For<_IInvocation>();
            var context = new Dictionary<string, object>
            {
                [Global.__flatwhite_outputcache_attribute] = new OutputCacheAttribute
                {
                    CacheStoreId = 100,
                    CacheStoreType = Substitute.For<ICacheStore>().GetType()
                }
            };
            var cacheStoreProvider = Substitute.For<ICacheStoreProvider>();
            cacheStoreProvider.GetCacheStore(100).Returns((ICacheStore)null);
            Global.CacheStoreProvider = cacheStoreProvider;

            var stg = new DefaultCacheStrategy();

            // Action
            var store = stg.GetCacheStore(invocation, context);

            // Assert
            cacheStoreProvider.Received(1).GetCacheStore(Arg.Is<Type>(t => t == Substitute.For<ICacheStore>().GetType()));
        }


        [Test]
        public void GetCacheStore_should_return_default_cache_store_if_not_found()
        {
            var invocation = Substitute.For<_IInvocation>();
            var context = new Dictionary<string, object>
            {
                [Global.__flatwhite_outputcache_attribute] = new OutputCacheAttribute
                {
                    CacheStoreId = 100,
                    CacheStoreType = Substitute.For<ICacheStore>().GetType()
                }
            };
            var cacheStoreProvider = Substitute.For<ICacheStoreProvider>();
            cacheStoreProvider.GetCacheStore(Arg.Is<int>(i => i > 0)).Returns((ICacheStore)null);
            cacheStoreProvider.GetCacheStore(Arg.Any<Type>()).Returns((ICacheStore)null);
            Global.CacheStoreProvider = cacheStoreProvider;

            var stg = new DefaultCacheStrategy();

            // Action
            var store = stg.GetCacheStore(invocation, context);

            // Assert
            cacheStoreProvider.Received(1).GetCacheStore(0);
        }


        [Test]
        public void GetAsyncCacheStore_should_return_found_async_store_by_id()
        {
            var invocation = Substitute.For<_IInvocation>();
            var context = new Dictionary<string, object>
            {
                [Global.__flatwhite_outputcache_attribute] = new OutputCacheAttribute
                {
                    CacheStoreId = 100
                }
            };
            var cacheStoreProvider = Substitute.For<ICacheStoreProvider>();
            cacheStoreProvider.GetAsyncCacheStore(100).Returns(Substitute.For<IAsyncCacheStore>());
            Global.CacheStoreProvider = cacheStoreProvider;

            var stg = new DefaultCacheStrategy();

            // Action
            var store = stg.GetAsyncCacheStore(invocation, context);

            // Assert
            cacheStoreProvider.Received(1).GetAsyncCacheStore(100);
            cacheStoreProvider.DidNotReceive().GetAsyncCacheStore(0);
        }

        [Test]
        public void GetAsyncCacheStore_should_try_to_get_by_type()
        {
            var invocation = Substitute.For<_IInvocation>();
            var context = new Dictionary<string, object>
            {
                [Global.__flatwhite_outputcache_attribute] = new OutputCacheAttribute
                {
                    CacheStoreId = 101,
                    CacheStoreType = Substitute.For<IAsyncCacheStore>().GetType()
                }
            };
            var cacheStoreProvider = Substitute.For<ICacheStoreProvider>();
            cacheStoreProvider.GetAsyncCacheStore(Arg.Any<int>()).Returns((IAsyncCacheStore)null);
            cacheStoreProvider.GetAsyncCacheStore(Arg.Any<Type>()).Returns(Substitute.For<IAsyncCacheStore>());
            Global.CacheStoreProvider = cacheStoreProvider;

            var stg = new DefaultCacheStrategy();

            // Action
            var store = stg.GetAsyncCacheStore(invocation, context);

            // Assert
            cacheStoreProvider.DidNotReceive().GetAsyncCacheStore(0);
        }

        [Test]
        public void GetAsyncCacheStore_by_type_should_handle_KeyNotFoundException()
        {
            var invocation = Substitute.For<_IInvocation>();
            var context = new Dictionary<string, object>
            {
                [Global.__flatwhite_outputcache_attribute] = new OutputCacheAttribute
                {
                    CacheStoreType = Substitute.For<IAsyncCacheStore>().GetType()
                }
            };
            var cacheStoreProvider = Substitute.For<ICacheStoreProvider>();
            cacheStoreProvider.When(x => x.GetAsyncCacheStore(Arg.Any<Type>())).Do(x => { throw new KeyNotFoundException(); });
            Global.CacheStoreProvider = cacheStoreProvider;

            var stg = new DefaultCacheStrategy();

            // Action
            var store = stg.GetAsyncCacheStore(invocation, context);

            // Assert
            cacheStoreProvider.Received(1).GetAsyncCacheStore(0);
        }

        [Test]
        public void GetAsyncCacheStore_by_type_should_not_return_null_if_cannot_resolve_by_type()
        {
            var invocation = Substitute.For<_IInvocation>();
            var context = new Dictionary<string, object>
            {
                [Global.__flatwhite_outputcache_attribute] = new OutputCacheAttribute
                {
                    CacheStoreType = Substitute.For<IAsyncCacheStore>().GetType()
                }
            };
            var cacheStoreProvider = Substitute.For<ICacheStoreProvider>();
            cacheStoreProvider.GetAsyncCacheStore(Arg.Any<Type>()).Returns((IAsyncCacheStore)null);
            Global.CacheStoreProvider = cacheStoreProvider;

            var stg = new DefaultCacheStrategy();

            // Action
            var store = stg.GetAsyncCacheStore(invocation, context);

            // Assert
            cacheStoreProvider.Received(1).GetAsyncCacheStore(0);
        }

        [Test]
        public void GetAsyncCacheStore_by_id_should_handle_Exception()
        {
            var invocation = Substitute.For<_IInvocation>();
            var context = new Dictionary<string, object>
            {
                [Global.__flatwhite_outputcache_attribute] = new OutputCacheAttribute
                {
                    CacheStoreId = 101,
                    CacheStoreType = Substitute.For<IAsyncCacheStore>().GetType()
                }
            };
            var cacheStoreProvider = Substitute.For<ICacheStoreProvider>();
            cacheStoreProvider.When(x => x.GetAsyncCacheStore(101)).Do(x => { throw new Exception(); });
            cacheStoreProvider.GetAsyncCacheStore(Arg.Any<Type>()).Returns(Substitute.For<IAsyncCacheStore>());
            Global.CacheStoreProvider = cacheStoreProvider;

            var stg = new DefaultCacheStrategy();

            // Action
            var store = stg.GetAsyncCacheStore(invocation, context);

            // Assert
            cacheStoreProvider.DidNotReceive().GetAsyncCacheStore(0);
        }

        [Test]
        public void GetAsyncCacheStore_should_try_to_get_the_cache_store_adaptor_by_store_type()
        {
            var invocation = Substitute.For<_IInvocation>();
            var context = new Dictionary<string, object>
            {
                [Global.__flatwhite_outputcache_attribute] = new OutputCacheAttribute
                {
                    CacheStoreType = Substitute.For<ICacheStore>().GetType()
                }
            };
            var cacheStoreProvider = Substitute.For<ICacheStoreProvider>();
            cacheStoreProvider.GetAsyncCacheStore(Arg.Any<int>()).Returns((IAsyncCacheStore)null);
            cacheStoreProvider.When(x => x.GetAsyncCacheStore(Arg.Is<Type>(t => typeof(IAsyncCacheStore).IsAssignableFrom(t)))).Do(x => { throw new KeyNotFoundException(); });
            Global.CacheStoreProvider = cacheStoreProvider;

            var stg = new DefaultCacheStrategy();

            // Action
            var store = stg.GetAsyncCacheStore(invocation, context);

            // Assert
            cacheStoreProvider.Received(1).GetCacheStore(Arg.Is<Type>(t => t == Substitute.For<ICacheStore>().GetType()));
            cacheStoreProvider.DidNotReceive().GetAsyncCacheStore(0);
            Assert.IsTrue(store is CacheStoreAdaptor);
        }

        [Test]
        public void GetAsyncCacheStore_should_not_return_null_if_cannot_get_ICacheStore_by_type()
        {
            var invocation = Substitute.For<_IInvocation>();
            var context = new Dictionary<string, object>
            {
                [Global.__flatwhite_outputcache_attribute] = new OutputCacheAttribute
                {
                    CacheStoreType = Substitute.For<ICacheStore>().GetType()
                }
            };
            var cacheStoreProvider = Substitute.For<ICacheStoreProvider>();
            cacheStoreProvider.When(x => x.GetAsyncCacheStore(Arg.Is<Type>(t => typeof(IAsyncCacheStore).IsAssignableFrom(t)))).Do(x => { throw new KeyNotFoundException(); });
            cacheStoreProvider.GetCacheStore(Arg.Is<Type>(t => typeof (ICacheStore).IsAssignableFrom(t))).Returns((ICacheStore)null);
            Global.CacheStoreProvider = cacheStoreProvider;

            var stg = new DefaultCacheStrategy();

            // Action
            var store = stg.GetAsyncCacheStore(invocation, context);

            // Assert
            cacheStoreProvider.Received(1).GetCacheStore(Arg.Is<Type>(t => t == Substitute.For<ICacheStore>().GetType()));
            cacheStoreProvider.Received(1).GetAsyncCacheStore(0);
            Assert.IsFalse(store is CacheStoreAdaptor);
        }

        [Test]
        public void GetAsyncCacheStore_should_return_default_store_if_cannot_get_store_adaptor()
        {
            var invocation = Substitute.For<_IInvocation>();
            var context = new Dictionary<string, object>
            {
                [Global.__flatwhite_outputcache_attribute] = new OutputCacheAttribute
                {
                    CacheStoreType = Substitute.For<ICacheStore>().GetType()
                }
            };
            var cacheStoreProvider = Substitute.For<ICacheStoreProvider>();
            cacheStoreProvider.When(x => x.GetAsyncCacheStore(Arg.Is<Type>(t => typeof(IAsyncCacheStore).IsAssignableFrom(t)))).Do(x => { throw new KeyNotFoundException(); });
            cacheStoreProvider.When(x => x.GetCacheStore(Arg.Is<Type>(t => typeof(ICacheStore).IsAssignableFrom(t)))).Do(x => { throw new KeyNotFoundException(); });
            Global.CacheStoreProvider = cacheStoreProvider;

            var stg = new DefaultCacheStrategy();

            // Action
            var store = stg.GetAsyncCacheStore(invocation, context);

            // Assert
            cacheStoreProvider.Received(1).GetCacheStore(Arg.Is<Type>(t => t == Substitute.For<ICacheStore>().GetType()));
            cacheStoreProvider.Received(1).GetAsyncCacheStore(0);
            Assert.IsFalse(store is CacheStoreAdaptor);
        }
    }
}
