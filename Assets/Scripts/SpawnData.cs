using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

// stage_data.json 한 줄(항목)에 대응하는 데이터 구조입니다.
// JSON 배열 전체 → SpawnData[] 배열 로 변환됩니다.
public class SpawnData
{
    // 이전 스폰으로부터 몇 초 뒤에 이 적을 생성할지
    public float delay;

    // JSON의 "type" 키 ("A" / "B" / "C" 문자열)를
    // Enemy.EnemyType 열거형(A/B/C)으로 자동 변환해줍니다
    [JsonProperty("type")]
    [JsonConverter(typeof(StringEnumConverter))]
    public Enemy.EnemyType enemyType;

    // spawnPoints 배열의 몇 번 위치에서 등장할지 (0, 1, 2 ...)
    public int point;
}
