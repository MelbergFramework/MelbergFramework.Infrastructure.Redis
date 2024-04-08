using System.Net;
using StackExchange.Redis;

namespace MelbergFramework.ComponentTesting.Redis.Mocks;

public class MockDatabaseAsync : IDatabaseAsync
{
    public Dictionary<RedisKey,MockRedisValue> _dictionary = new();

    public IConnectionMultiplexer Multiplexer => throw new NotImplementedException();

    public MockDatabaseAsync() { }

    private void ApplyTime(RedisKey key)
    {
        if(_dictionary.TryGetValue(key, out var value ))
        {
            if(value.TimeOfDeath < DateTime.UtcNow)
            {
                _dictionary.Remove(key);
            }
        }
    }

    private MockRedisValue GetResultEnforceExpectations(RedisKey key, bool shouldBeSet)
    {
        ApplyTime(key);
        var result = _dictionary[key];
        if(result.IsSet && !shouldBeSet)
        {
            throw new Exception("Placeholder");
        }

        return result;
    }

    public Task<bool> LockReleaseAsync(
            RedisKey key,
            RedisValue value,
            CommandFlags flags = CommandFlags.None)
    {
        ApplyTime(key);
        var result = _dictionary[key];
        if(result.Values.First() != value)
        {
            return Task.FromResult(false);
        }
        result.IsLocked = false;
        _dictionary[key] = result;

        return Task.FromResult(true);
    }

    public Task<bool> LockTakeAsync(
            RedisKey key,
            RedisValue value,
            TimeSpan expiry,
            CommandFlags flags = CommandFlags.None)
    {
        ApplyTime(key);
        if(_dictionary.ContainsKey(key))
        {
            var currentvalue = _dictionary[key];
            if(currentvalue.IsLocked)
            {
                return Task.FromResult(false);
            }
        }
        
        _dictionary[key] = new MockRedisValue(value);

        _dictionary[key].IsLocked = true;
        _dictionary[key].TimeOfDeath = DateTime.UtcNow.Add(expiry);


        return Task.FromResult(true);
    }


    public Task<RedisValue> StringGetAsync(
            RedisKey key,
            CommandFlags flags = CommandFlags.None)
    {
        ApplyTime(key);
        var result = GetResultEnforceExpectations(key, false);
        return Task.FromResult(result.Values.First());
    }

    public Task<RedisValue[]> StringGetAsync(
            RedisKey[] keys,
            CommandFlags flags = CommandFlags.None)
    {
        return Task.FromResult( 
            keys
                .Select(_ => GetResultEnforceExpectations(_, false)
                                .Values
                                .First())
                .ToArray());
    }

    public Task<bool> StringSetAsync(
            RedisKey key,
            RedisValue value,
            TimeSpan? expiry = null,
            When when = When.Always,
            CommandFlags flags = CommandFlags.None)
    {
        ApplyTime(key);
        if(when != When.Always)
        {
            throw new Exception("when must be Always");
        }

        if(flags != CommandFlags.None)
        {
            throw new Exception("flags must be None");
        }

        _dictionary[key] = new MockRedisValue(value);
        return Task.FromResult(true);
    }

    public async Task<bool> StringSetAsync(
            KeyValuePair<RedisKey,
            RedisValue>[] values,
            When when = When.Always,
            CommandFlags flags = CommandFlags.None)
    {
        foreach(var value in values)
        {
            ApplyTime(value.Key);
            await StringSetAsync(value.Key,value.Value);
        }

        return true;
    }

    public Task<RedisValue[]> SetMembersAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
    {
        ApplyTime(key);
        var result = GetResultEnforceExpectations(key,true);

        return Task.FromResult(result.Values.ToArray());
    }

    public Task<bool> SetAddAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
    {
        ApplyTime(key);

        if(!_dictionary.TryGetValue(key, out var result))
        {
            _dictionary[key] = new MockRedisValue(value){IsSet = true};
            return Task.FromResult(true);
        }

        result.Values.Add(value);

        _dictionary[key] = result;

        return Task.FromResult(true);
    }

    public Task<long> SetAddAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<bool> KeyExpireAsync(RedisKey key, TimeSpan? expiry, CommandFlags flags = CommandFlags.None)
    {
        ApplyTime(key);
        _dictionary[key].TimeOfDeath = DateTime.UtcNow.Add(expiry ?? TimeSpan.Zero);
        return Task.FromResult(true);
    }

    public Task<bool> KeyExpireAsync(RedisKey key, DateTime? expiry, CommandFlags flags = CommandFlags.None)
    {
        ApplyTime(key);
        _dictionary[key].TimeOfDeath = expiry ?? DateTime.MaxValue;
        return Task.FromResult(true);
    }

    public Task<long> ListRightPushAsync(RedisKey key, RedisValue[] values, CommandFlags flags)
    {
        ApplyTime(key);
        var result = GetResultEnforceExpectations(key,true);

        result.Values.AddRange(values);

        _dictionary[key] = result;

        return Task.FromResult((long)0);

    }

    public Task<RedisValue[]> ListRangeAsync(RedisKey key, long start = 0, long stop = -1, CommandFlags flags = CommandFlags.None)
    {
        ApplyTime(key);
        var result = GetResultEnforceExpectations(key,true);

        return Task.FromResult(
                result
                    .Values
                    .GetRange((int)start, (int)stop)
                    .ToArray());
    }


    public Task<RedisValue> DebugObjectAsync(RedisKey key, CommandFlags flags = CommandFlags.None) { throw new NotImplementedException(); }

    public Task<RedisResult> ExecuteAsync(string command, params object[] args) { throw new NotImplementedException(); }

    public Task<RedisResult> ExecuteAsync(string command, ICollection<object> args, CommandFlags flags = CommandFlags.None) { throw new NotImplementedException(); }

    public Task<bool> GeoAddAsync(RedisKey key, double longitude, double latitude, RedisValue member, CommandFlags flags = CommandFlags.None) { throw new NotImplementedException(); }

    public Task<bool> GeoAddAsync(RedisKey key, GeoEntry value, CommandFlags flags = CommandFlags.None) { throw new NotImplementedException(); }

    public Task<long> GeoAddAsync(RedisKey key, GeoEntry[] values, CommandFlags flags = CommandFlags.None) { throw new NotImplementedException(); }

    public Task<double?> GeoDistanceAsync(RedisKey key, RedisValue member1, RedisValue member2, GeoUnit unit = GeoUnit.Meters, CommandFlags flags = CommandFlags.None) { throw new NotImplementedException(); }

    public Task<string[]> GeoHashAsync(RedisKey key, RedisValue[] members, CommandFlags flags = CommandFlags.None) { throw new NotImplementedException(); }

    public Task<string> GeoHashAsync(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None) { throw new NotImplementedException(); }

    public Task<GeoPosition?[]> GeoPositionAsync(RedisKey key, RedisValue[] members, CommandFlags flags = CommandFlags.None) { throw new NotImplementedException(); }

    public Task<GeoPosition?> GeoPositionAsync(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None) { throw new NotImplementedException(); }

    public Task<GeoRadiusResult[]> GeoRadiusAsync(RedisKey key, RedisValue member, double radius, GeoUnit unit = GeoUnit.Meters, int count = -1, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.Default, CommandFlags flags = CommandFlags.None) { throw new NotImplementedException(); }

    public Task<GeoRadiusResult[]> GeoRadiusAsync(RedisKey key, double longitude, double latitude, double radius, GeoUnit unit = GeoUnit.Meters, int count = -1, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.Default, CommandFlags flags = CommandFlags.None) { throw new NotImplementedException(); }

    public Task<bool> GeoRemoveAsync(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None) { throw new NotImplementedException(); }

    public Task<long> HashDecrementAsync(RedisKey key, RedisValue hashField, long value = 1, CommandFlags flags = CommandFlags.None) { throw new NotImplementedException(); }

    public Task<double> HashDecrementAsync(RedisKey key, RedisValue hashField, double value, CommandFlags flags = CommandFlags.None) { throw new NotImplementedException(); }

    public Task<bool> HashDeleteAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None) { throw new NotImplementedException(); }

    public Task<long> HashDeleteAsync(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None) { throw new NotImplementedException(); }

    public Task<bool> HashExistsAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None) { throw new NotImplementedException(); }

    public Task<HashEntry[]> HashGetAllAsync(RedisKey key, CommandFlags flags = CommandFlags.None) { throw new NotImplementedException(); }

    public Task<RedisValue> HashGetAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None) { throw new NotImplementedException(); } public Task<RedisValue[]> HashGetAsync(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None) { throw new NotImplementedException(); } public Task<Lease<byte>> HashGetLeaseAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None) { throw new NotImplementedException(); }

    public Task<long> HashIncrementAsync(RedisKey key, RedisValue hashField, long value = 1, CommandFlags flags = CommandFlags.None) { throw new NotImplementedException(); }

    public Task<double> HashIncrementAsync(RedisKey key, RedisValue hashField, double value, CommandFlags flags = CommandFlags.None) { throw new NotImplementedException(); }

    public Task<RedisValue[]> HashKeysAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<long> HashLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<HashEntry> HashScanAsync(RedisKey key, RedisValue pattern = default, int pageSize = 250, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task HashSetAsync(RedisKey key, HashEntry[] hashFields, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<bool> HashSetAsync(RedisKey key, RedisValue hashField, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<long> HashStringLengthAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<RedisValue[]> HashValuesAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<bool> HyperLogLogAddAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<bool> HyperLogLogAddAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<long> HyperLogLogLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<long> HyperLogLogLengthAsync(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task HyperLogLogMergeAsync(RedisKey destination, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task HyperLogLogMergeAsync(RedisKey destination, RedisKey[] sourceKeys, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<EndPoint> IdentifyEndpointAsync(RedisKey key = default, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public bool IsConnected(RedisKey key, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<bool> KeyDeleteAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<long> KeyDeleteAsync(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<byte[]> KeyDumpAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<bool> KeyExistsAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<long> KeyExistsAsync(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<TimeSpan?> KeyIdleTimeAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task KeyMigrateAsync(RedisKey key, EndPoint toServer, int toDatabase = 0, int timeoutMilliseconds = 0, MigrateOptions migrateOptions = MigrateOptions.None, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<bool> KeyMoveAsync(RedisKey key, int database, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<bool> KeyPersistAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<RedisKey> KeyRandomAsync(CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<bool> KeyRenameAsync(RedisKey key, RedisKey newKey, When when = When.Always, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task KeyRestoreAsync(RedisKey key, byte[] value, TimeSpan? expiry = null, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<TimeSpan?> KeyTimeToLiveAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<bool> KeyTouchAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<long> KeyTouchAsync(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<RedisType> KeyTypeAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<RedisValue> ListGetByIndexAsync(RedisKey key, long index, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<long> ListInsertAfterAsync(RedisKey key, RedisValue pivot, RedisValue value, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<long> ListInsertBeforeAsync(RedisKey key, RedisValue pivot, RedisValue value, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<RedisValue> ListLeftPopAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<RedisValue[]> ListLeftPopAsync(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<long> ListLeftPushAsync(RedisKey key, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<long> ListLeftPushAsync(RedisKey key, RedisValue[] values, When when = When.Always, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<long> ListLeftPushAsync(RedisKey key, RedisValue[] values, CommandFlags flags)
    {
        throw new NotImplementedException();
    }

    public Task<long> ListLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }


    public Task<long> ListRemoveAsync(RedisKey key, RedisValue value, long count = 0, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<RedisValue> ListRightPopAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<RedisValue[]> ListRightPopAsync(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<RedisValue> ListRightPopLeftPushAsync(RedisKey source, RedisKey destination, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<long> ListRightPushAsync(RedisKey key, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<long> ListRightPushAsync(RedisKey key, RedisValue[] values, When when = When.Always, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task ListSetByIndexAsync(RedisKey key, long index, RedisValue value, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task ListTrimAsync(RedisKey key, long start, long stop, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<bool> LockExtendAsync(RedisKey key, RedisValue value, TimeSpan expiry, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<RedisValue> LockQueryAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<TimeSpan> PingAsync(CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<long> PublishAsync(RedisChannel channel, RedisValue message, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<RedisResult> ScriptEvaluateAsync(string script, RedisKey[] keys = null, RedisValue[] values = null, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<RedisResult> ScriptEvaluateAsync(byte[] hash, RedisKey[] keys = null, RedisValue[] values = null, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<RedisResult> ScriptEvaluateAsync(LuaScript script, object parameters = null, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<RedisResult> ScriptEvaluateAsync(LoadedLuaScript script, object parameters = null, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }


    public Task<long> SetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<long> SetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<RedisValue[]> SetCombineAsync(SetOperation operation, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<RedisValue[]> SetCombineAsync(SetOperation operation, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<bool> SetContainsAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<long> SetLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }


    public Task<bool> SetMoveAsync(RedisKey source, RedisKey destination, RedisValue value, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<RedisValue> SetPopAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<RedisValue[]> SetPopAsync(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<RedisValue> SetRandomMemberAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<RedisValue[]> SetRandomMembersAsync(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<bool> SetRemoveAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<long> SetRemoveAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<RedisValue> SetScanAsync(RedisKey key, RedisValue pattern = default, int pageSize = 250, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<long> SortAndStoreAsync(RedisKey destination, RedisKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, RedisValue by = default, RedisValue[] get = null, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<RedisValue[]> SortAsync(RedisKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, RedisValue by = default, RedisValue[] get = null, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<bool> SortedSetAddAsync(RedisKey key, RedisValue member, double score, CommandFlags flags)
    {
        throw new NotImplementedException();
    }

    public Task<bool> SortedSetAddAsync(RedisKey key, RedisValue member, double score, When when = When.Always, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<long> SortedSetAddAsync(RedisKey key, SortedSetEntry[] values, CommandFlags flags)
    {
        throw new NotImplementedException();
    }

    public Task<long> SortedSetAddAsync(RedisKey key, SortedSetEntry[] values, When when = When.Always, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<long> SortedSetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey first, RedisKey second, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<long> SortedSetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey[] keys, double[] weights = null, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<double> SortedSetDecrementAsync(RedisKey key, RedisValue member, double value, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<double> SortedSetIncrementAsync(RedisKey key, RedisValue member, double value, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<long> SortedSetLengthAsync(RedisKey key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<long> SortedSetLengthByValueAsync(RedisKey key, RedisValue min, RedisValue max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<SortedSetEntry?> SortedSetPopAsync(RedisKey key, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<SortedSetEntry[]> SortedSetPopAsync(RedisKey key, long count, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<RedisValue[]> SortedSetRangeByRankAsync(RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<SortedSetEntry[]> SortedSetRangeByRankWithScoresAsync(RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<RedisValue[]> SortedSetRangeByScoreAsync(RedisKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<SortedSetEntry[]> SortedSetRangeByScoreWithScoresAsync(RedisKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<RedisValue[]> SortedSetRangeByValueAsync(RedisKey key, RedisValue min, RedisValue max, Exclude exclude, long skip, long take = -1, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<RedisValue[]> SortedSetRangeByValueAsync(RedisKey key, RedisValue min = default, RedisValue max = default, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<long?> SortedSetRankAsync(RedisKey key, RedisValue member, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<bool> SortedSetRemoveAsync(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<long> SortedSetRemoveAsync(RedisKey key, RedisValue[] members, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<long> SortedSetRemoveRangeByRankAsync(RedisKey key, long start, long stop, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<long> SortedSetRemoveRangeByScoreAsync(RedisKey key, double start, double stop, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<long> SortedSetRemoveRangeByValueAsync(RedisKey key, RedisValue min, RedisValue max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<SortedSetEntry> SortedSetScanAsync(RedisKey key, RedisValue pattern = default, int pageSize = 250, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<double?> SortedSetScoreAsync(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<long> StreamAcknowledgeAsync(RedisKey key, RedisValue groupName, RedisValue messageId, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<long> StreamAcknowledgeAsync(RedisKey key, RedisValue groupName, RedisValue[] messageIds, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<RedisValue> StreamAddAsync(RedisKey key, RedisValue streamField, RedisValue streamValue, RedisValue? messageId = null, int? maxLength = null, bool useApproximateMaxLength = false, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<RedisValue> StreamAddAsync(RedisKey key, NameValueEntry[] streamPairs, RedisValue? messageId = null, int? maxLength = null, bool useApproximateMaxLength = false, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<StreamEntry[]> StreamClaimAsync(RedisKey key, RedisValue consumerGroup, RedisValue claimingConsumer, long minIdleTimeInMs, RedisValue[] messageIds, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<RedisValue[]> StreamClaimIdsOnlyAsync(RedisKey key, RedisValue consumerGroup, RedisValue claimingConsumer, long minIdleTimeInMs, RedisValue[] messageIds, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<bool> StreamConsumerGroupSetPositionAsync(RedisKey key, RedisValue groupName, RedisValue position, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<StreamConsumerInfo[]> StreamConsumerInfoAsync(RedisKey key, RedisValue groupName, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<bool> StreamCreateConsumerGroupAsync(RedisKey key, RedisValue groupName, RedisValue? position, CommandFlags flags)
    {
        throw new NotImplementedException();
    }

    public Task<bool> StreamCreateConsumerGroupAsync(RedisKey key, RedisValue groupName, RedisValue? position = null, bool createStream = true, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<long> StreamDeleteAsync(RedisKey key, RedisValue[] messageIds, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<long> StreamDeleteConsumerAsync(RedisKey key, RedisValue groupName, RedisValue consumerName, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<bool> StreamDeleteConsumerGroupAsync(RedisKey key, RedisValue groupName, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<StreamGroupInfo[]> StreamGroupInfoAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<StreamInfo> StreamInfoAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<long> StreamLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<StreamPendingInfo> StreamPendingAsync(RedisKey key, RedisValue groupName, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<StreamPendingMessageInfo[]> StreamPendingMessagesAsync(RedisKey key, RedisValue groupName, int count, RedisValue consumerName, RedisValue? minId = null, RedisValue? maxId = null, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<StreamEntry[]> StreamRangeAsync(RedisKey key, RedisValue? minId = null, RedisValue? maxId = null, int? count = null, Order messageOrder = Order.Ascending, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<StreamEntry[]> StreamReadAsync(RedisKey key, RedisValue position, int? count = null, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<RedisStream[]> StreamReadAsync(StreamPosition[] streamPositions, int? countPerStream = null, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<StreamEntry[]> StreamReadGroupAsync(RedisKey key, RedisValue groupName, RedisValue consumerName, RedisValue? position, int? count, CommandFlags flags)
    {
        throw new NotImplementedException();
    }

    public Task<StreamEntry[]> StreamReadGroupAsync(RedisKey key, RedisValue groupName, RedisValue consumerName, RedisValue? position = null, int? count = null, bool noAck = false, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<RedisStream[]> StreamReadGroupAsync(StreamPosition[] streamPositions, RedisValue groupName, RedisValue consumerName, int? countPerStream, CommandFlags flags)
    {
        throw new NotImplementedException();
    }

    public Task<RedisStream[]> StreamReadGroupAsync(StreamPosition[] streamPositions, RedisValue groupName, RedisValue consumerName, int? countPerStream = null, bool noAck = false, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<long> StreamTrimAsync(RedisKey key, int maxLength, bool useApproximateMaxLength = false, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<long> StringAppendAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<long> StringBitCountAsync(RedisKey key, long start = 0, long end = -1, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<long> StringBitOperationAsync(Bitwise operation, RedisKey destination, RedisKey first, RedisKey second = default, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<long> StringBitOperationAsync(Bitwise operation, RedisKey destination, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<long> StringBitPositionAsync(RedisKey key, bool bit, long start = 0, long end = -1, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<long> StringDecrementAsync(RedisKey key, long value = 1, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<double> StringDecrementAsync(RedisKey key, double value, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<bool> StringGetBitAsync(RedisKey key, long offset, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<RedisValue> StringGetDeleteAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<Lease<byte>> StringGetLeaseAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<RedisValue> StringGetRangeAsync(RedisKey key, long start, long end, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<RedisValue> StringGetSetAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<RedisValueWithExpiry> StringGetWithExpiryAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<long> StringIncrementAsync(RedisKey key, long value = 1, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<double> StringIncrementAsync(RedisKey key, double value, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<long> StringLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }



    public Task<bool> StringSetBitAsync(RedisKey key, long offset, bool bit, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task<RedisValue> StringSetRangeAsync(RedisKey key, long offset, RedisValue value, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public bool TryWait(Task task) { throw new NotImplementedException(); }

    public void Wait(Task task) { throw new NotImplementedException(); }

    public T Wait<T>(Task<T> task) { throw new NotImplementedException(); }

    public void WaitAll(params Task[] tasks) { throw new NotImplementedException(); }
}
