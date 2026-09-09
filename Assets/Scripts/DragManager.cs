using UnityEngine;

public class DragManager : MonoBehaviour
{
    [Header("Cài đặt Hệ thống")]
    public GameObject[] turretPrefabs; // Chứa 20 level súng (Từ cùi đến xịn)
    public Slot[] allSlots;            // Chứa toàn bộ 20 ô (5 combat + 15 merge)

    [Header("Tinh chỉnh vị trí xuất hiện của Súng")]
    public Vector3 spawnOffset = new Vector3(0, 0.15f, 0); // Chỉnh Y cao/thấp tùy ý trên Inspector

    private Camera cam;
    private Turret selectedTurret;
    private Slot originalSlot;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        // ==========================================
        // [TEST] BẤM SPACE ĐỂ GỌI SÚNG VÀO Ô TRỐNG
        // ==========================================
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnTestTurret();
        }

        // ==========================================
        // GIAI ĐOẠN 1: BẤM CHUỘT -> NHẶT SÚNG
        // ==========================================
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                Turret clickedTurret = hit.collider.GetComponent<Turret>();
                if (clickedTurret != null)
                {
                    selectedTurret = clickedTurret;
                    originalSlot = selectedTurret.currentSlot;
                }
            }
        }

        // ==========================================
        // GIAI ĐOẠN 2: GIỮ CHUỘT -> KÉO SÚNG THEO
        // ==========================================
        if (Input.GetMouseButton(0) && selectedTurret != null)
        {
            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            selectedTurret.transform.position = mousePos;
        }

        // ==========================================
        // GIAI ĐOẠN 3: THẢ CHUỘT -> XỬ LÝ GỘP / ĐỔI CHỖ
        // ==========================================
        if (Input.GetMouseButtonUp(0) && selectedTurret != null)
        {
            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

            // Quét xem chuột có đang nằm trên object nào thuộc Layer "Slot" không
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Slot"));

            if (hit.collider != null)
            {
                Slot targetSlot = hit.collider.GetComponent<Slot>();
                if (targetSlot != null)
                {
                    HandleDrop(targetSlot);
                }
                else ReturnToOriginalSlot();
            }
            else ReturnToOriginalSlot(); // Trượt ra ngoài Grid -> Bay về nhà

            selectedTurret = null; // Xong việc thì reset bàn tay lại
        }
    }

    // --- HÀM XỬ LÝ LOGIC KHI THẢ SÚNG VÀO Ô ---
    void HandleDrop(Slot targetSlot)
    {
        // 1. Nếu thả về lại chính cái ô lúc nãy vừa nhấc lên
        if (targetSlot == originalSlot)
        {
            ReturnToOriginalSlot();
            return;
        }

        // 2. Nếu thả vào Ô TRỐNG
        if (targetSlot.IsEmpty())
        {
            originalSlot.currentTurret = null;
            targetSlot.currentTurret = selectedTurret;
            selectedTurret.currentSlot = targetSlot;

            // Đặt súng vào ô có cộng thêm Offset tinh chỉnh
            selectedTurret.transform.position = targetSlot.transform.position + spawnOffset;
            selectedTurret.laneID = targetSlot.slotLaneID;
        }
        // 3. Nếu thả vào Ô ĐÃ CÓ SÚNG SẴN
        else
        {
            Turret targetTurret = targetSlot.currentTurret;

            // TRƯỜNG HỢP 3A: CÙNG LEVEL -> GỘP (MERGE)
            if (targetTurret.turretLevel == selectedTurret.turretLevel)
            {
                originalSlot.currentTurret = null;
                int nextLevel = targetTurret.turretLevel; // Level tiếp theo

                // Xóa 2 súng cũ
                Destroy(selectedTurret.gameObject);
                Destroy(targetTurret.gameObject);

                // Sinh súng mới (Tránh lỗi vượt quá mảng nếu ghép súng max level)
                if (nextLevel < turretPrefabs.Length)
                {
                    // Vị trí đẻ súng mới có cộng thêm Offset
                    Vector3 spawnPos = targetSlot.transform.position + spawnOffset;
                    GameObject newTurretObj = Instantiate(turretPrefabs[nextLevel], spawnPos, Quaternion.identity);
                    Turret newTurret = newTurretObj.GetComponent<Turret>();

                    newTurret.currentSlot = targetSlot;
                    targetSlot.currentTurret = newTurret;
                    newTurret.laneID = targetSlot.slotLaneID;
                }
            }
            // TRƯỜNG HỢP 3B: KHÁC LEVEL -> ĐỔI CHỖ (SWAP)
            else
            {
                // Súng dưới ô bay về nhà cũ (có cộng Offset)
                originalSlot.currentTurret = targetTurret;
                targetTurret.currentSlot = originalSlot;
                targetTurret.transform.position = originalSlot.transform.position + spawnOffset;
                targetTurret.laneID = originalSlot.slotLaneID;

                // Súng trên tay rớt xuống ô mới (có cộng Offset)
                targetSlot.currentTurret = selectedTurret;
                selectedTurret.currentSlot = targetSlot;
                selectedTurret.transform.position = targetSlot.transform.position + spawnOffset;
                selectedTurret.laneID = targetSlot.slotLaneID;
            }
        }
    }

    // --- HÀM CHO SÚNG BAY VỀ CHỖ CŨ NẾU LÀM RỚT ---
    void ReturnToOriginalSlot()
    {
        // Khi bay về nhà cũ cũng áp dụng offset để khớp vị trí
        selectedTurret.transform.position = originalSlot.transform.position + spawnOffset;
    }

    // --- HÀM TEST: TỰ ĐỘNG ĐẺ SÚNG VÀO Ô TRỐNG KHI BẤM SPACE ---
    void SpawnTestTurret()
    {
        foreach (Slot slot in allSlots)
        {
            if (slot.slotLaneID == -1 && slot.IsEmpty())
            {
                // Vị trí đẻ súng test có cộng thêm Offset
                Vector3 spawnPos = slot.transform.position + spawnOffset;
                GameObject newTurretObj = Instantiate(turretPrefabs[0], spawnPos, Quaternion.identity);
                Turret newTurret = newTurretObj.GetComponent<Turret>();

                newTurret.currentSlot = slot;
                slot.currentTurret = newTurret;
                newTurret.laneID = slot.slotLaneID;

                Debug.Log($"[Test] Đã đẻ 1 súng Level 1 tại ô: {slot.gameObject.name}");
                return;
            }
        }
        Debug.Log("[Test] Hết ô trống rồi, không đẻ được nữa!");
    }
}