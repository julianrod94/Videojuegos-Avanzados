using StateNS;
using UnityEngine;

namespace ChannelNS {
    public class PlayerPositionStateChannel: StateChannel<PlayerPositionState> {
        private float _positionMin = -100;
        private float _positionMax = 100;
        private float _positionPrecision = 0.1f;
        
        private float _timeStampMin = 0;
        private float _timeStampMax = 60;
        private float _timeStampPrecision = 1/60f;

        public PlayerPositionStateChannel(ISenderStrategy strategy) {
            setupStrategy(strategy);
        }
        
        public override PlayerPositionState DeserializeData(byte[] bytes) {
            buffer.ToRead();
            float x = buffer.ReadFloat(_positionMin, _positionMax, _positionPrecision);
            float y = buffer.ReadFloat(_positionMin, _positionMax, _positionPrecision);
            float z = buffer.ReadFloat(_positionMin, _positionMax, _positionPrecision);
            float timeStamp = buffer.ReadFloat(_timeStampMin, _timeStampMax, _timeStampPrecision);
            return new PlayerPositionState(new Vector3(x,y,z), timeStamp);
        }

        public override byte[] SerializeData(PlayerPositionState data) {
            buffer.ToWrite();
            buffer.WriteFloat(data.Position.x, _positionMin, _positionMax, _positionPrecision);
            buffer.WriteFloat(data.Position.y, _positionMin, _positionMax, _positionPrecision);
            buffer.WriteFloat(data.Position.z, _positionMin, _positionMax, _positionPrecision);
            buffer.WriteFloat(data.Position.z, _timeStampMin, _timeStampMax, _timeStampPrecision);
            return buffer.GenerateBytes();
        }
    }
}