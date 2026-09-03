# KatyushaPJ — Dialogue Script (bản mod: giảm dẫn chuyện, tăng thoại nhân vật)

> Bản này viết lại từ file cốt truyện gốc. Narrator chỉ giữ lại khi THỰC SỰ
> không thể gán cho ai nói (chuyển cảnh, time-skip). Mỗi dòng đánh dấu
> `[TênNhânVật]` để sau này map trực tiếp sang `CharacterProfile` khi tạo
> `DialogueData` asset trong Unity.

## Cast tham chiếu

| Tên trong game | Vai trò | Ghi chú portrait |
|---|---|---|
| Kati | Nhân vật chính, mèo | Cần portrait thường + có thể thêm biểu cảm khó chịu/bối rối (dùng nhiều) |
| Hachi | Linh hồn chiến binh, companion | Portrait dạng linh hồn không chân |
| Usagi | Thương nhân | Xuất hiện lặp lại Ch.1, 4, 6. Mỗi lần gặp gồm 2 phần: (1) trò chuyện dạng DialogueData như các NPC khác, (2) mở Shop UI — xem ghi chú cuối file |
| Kuri | Chủ trang trại | Ch.3 |
| Mika | Cận thần / chiến binh hoàng gia | Ch.7 |
| Kanus | Nhà vua (twist boss) | Ch.7 |
| Dân Làng | NPC không tên, vài dòng ngắn | Ch.1 |
| Narrator | CHỈ dùng tối thiểu, không portrait | Rải rác, cực ít |

---

## Chapter 1 — Khởi đầu

**Scene 1.1 — Khu rừng gần làng (mở đầu, chưa có Hachi để đối thoại)**

> Đây là đoạn duy nhất buộc phải giữ Narrator vì Kati đang một mình, chưa có
> ai để nói chuyện cùng.

- [Narrator]: Một buổi sáng bình thường như bao ngày khác, Kati tình cờ bắt gặp một chiếc rương kỳ lạ nằm giữa bụi cây.
- [Kati]: (gừ gừ, tò mò) Cái quái gì đây ta...

*(Kati cạy rương ra. Một tiếng nổ nhẹ vang lên, một bóng hình kỳ lạ thoát ra khỏi rương — trông như một linh hồn không chân, lơ lửng trên không.)*

- [Kati]: (gầm gừ cảnh giác) Gừrrr!!
- [Hachi]: Ôi... lâu lắm rồi tôi mới thấy được tự do như thế này. Không biết đã bao lâu rồi tôi mới được nhìn thấy thế giới bên ngoài.
- [Kati]: (vẫn cảnh giác, nhìn kỹ) ...Mặt mày giống mèo mà thân thì vỡ vụn kỳ dị vậy?
- [Hachi]: Chắc cậu là người đã giải thoát cho tôi đúng không? Cảm ơn cậu nhiều lắm. Xin lỗi vì lúc nãy làm cậu sợ — tôi tên Hachi, tôi đến từ một thế giới khác.
- [Kati]: C-cái gì? Thế giới khác?
- [Hachi]: Đúng vậy. Chuyện dài lắm, để tôi kể cho cậu nghe.

**Scene 1.2 — Hachi kể chuyện**

- [Hachi]: Để tôi kể gọn cho cậu nghe nè — tôi vốn là chiến binh tập sự của vương quốc Tanria, thế giới Eldzanha. Trẻ mồ côi chính hiệu, được thầy Shisa — pháp sư hoàng gia siêu xịn — nhận nuôi dạy dỗ.
- [Hachi]: Rồi một hôm xui tận mạng, lũ quỷ từ bên kia thế giới ập tới tấn công. Cả thủ đô thất thủ trong chớp mắt luôn á.
- [Hachi]: Lúc nguy cấp vậy đó, thầy Shisa tung ra một chiêu ông nghiên cứu cả đời — thuật dịch chuyển — định đưa tôi đi cầu cứu.
- [Hachi]: Nhưng mà phép chưa hoàn thiện, cộng thêm ma thuật của quỷ ăn sâu vào người thầy rồi, nên đường đi bị lệch tùm lum, văng tôi qua tận đây luôn. Xui chưa!
- [Kati]: (ngớ người) ...
- [Hachi]: Nè! Cậu giúp tôi giải thoát vương quốc của mình đi!
- [Kati]: Này đừng có điên. Tôi còn nhiều việc phải làm, không rảnh xen vào chuyện của cậu. Với lại tôi có sức mạnh gì đâu mà giúp.
- [Hachi]: À mà quên nói — linh hồn tôi đã gắn với cậu rồi đó.
- [Kati]: (giật mình) Hả?!
- [Hachi]: Lúc dịch chuyển hình như không kéo được cơ thể tôi theo, nên nó chỉ bị giam trong chiếc rương kia thôi. Nhưng cậu lỡ cạy rương ra rồi, linh hồn tôi tự động tìm thứ gần nhất để ký sinh vào... và đó là cơ thể cậu.
- [Hachi]: Muốn tách chúng ta ra, cách duy nhất là tìm lại cơ thể cũ của tôi.
- [Kati]: (điên tiết, vùng vẫy) THẢ TÔI RA! THẢ TÔI RA NGAY!

*(Sau một hồi cố tách Hachi ra bất thành, Kati đành bỏ cuộc.)*

- [Kati]: (thở dài) ...Được rồi được rồi, tôi chịu thua. Mà này, chúng ta tới thế giới của cậu bằng cách nào?
- [Hachi]: Không sao, tôi đang giữ một miếng mề đay — thứ này kết nối tới thế giới cũ của tôi. Đây là loại ma thuật đặc trưng của vương quốc tôi. Chưa thí nghiệm nhiều với việc dịch chuyển liên chiều, nhưng nếu tôi tới được đây thì quay lại chắc cũng không khó.

**Scene 1.3 — Đặt chân tới Eldzanha, gặp dân làng**

- [Narrator]: Bằng một cách nào đó may mắn, cả hai đặt chân tới Eldzanha — không phải thủ đô, mà là một vùng ngoại ô.
- [Kati]: (ngó quanh) Ừm... nhìn cũng bình thường mà? Đâu tệ như cậu tả.
- [Hachi]: (nhíu mày) Cậu đâu cảm nhận được ma lực, chỉ mình tôi thấy thôi — vẫn còn dư vị ghê ghê trong không khí này, nhưng đỡ hơn nhiều so với thủ đô rồi. Ít ra vùng này chưa bị chiếm đóng nặng. Đi tìm người sống sót thôi!

*(Họ tìm thấy một hang động nhỏ — nơi dân làng còn sống sót đang trú ẩn.)*

- [Hachi]: Nè Kati, dừng chân 1 chút đi, ngôi nhà này thật kì lạ.
- [Kati]: (ngó quanh) ...Kì lạ là sao? Cậu thấy điểm gì bất thường à?
- [Hachi]: tôi không rõ tuy nó rất yếu ớt nhưng mà hình như vẫn còn người sống ở đây.
- [Narrator](Teleaction xuống tọa độ 100, 100): cả 2 cùng tiến vào bên trong. Hachi tìm thấy được một hầm trú ẩn, cả hai bước vào và thấy một nhóm dân làng đang run rẩy trong bóng tối. Họ nhìn thấy Hachi và Kati, ánh mắt vừa sợ hãi vừa hy vọng.
- [Dân làng có vẻ sợ hãi]: Là quỷ sao?! Huhu mẹ ơi, con sắp chết rồi!
- [Dân làng có vẻ điềm tĩnh]: Không phải đâu, cậu ấy không phải quỷ, cậu ấy cùng giống loài với chúng ta đồ ngốc ạ. Bình tĩnh lại đi.
- [Hachi]: Tôi là Hachi, chiến binh hoàng gia, tuy có nhiều chuyện xảy ra khiến tôi chỉ còn lại linh hồn thôi nhưng tôi vẫn còn sức mạnh để giúp các cậu. Trước hết tôi cần biết, các cậu còn vũ khí gì không?
- [Dân làng có vẻ bình tĩnh]: Ngôi làng này chỉ là một làng nhỏ, nguồn sống chính của chúng tôi dựa vào nông nghiệp.
- [Dân làng có vẻ bình tĩnh]: Tôi là tên là Snake, người duy nhất ở đây biết một chút về chiến đấu và ma thuật. Phần lớn vũ khí của chúng tôi đều rất thô sơ, tôi chỉ có một con dao nhỏ thôi.
- [Snake]: Tuy không tốt như những thứ cậu từng sử dụng, nhưng đó là vũ khí diệt quỷ tốt nhất mà chúng tôi có ở đây.
- [Hachi]: Cảm ơn anh. Có vẻ anh là người dựng lên kết giới để hạn chế ma lực của dân làng này để ẩn nấp phải không? Anh yên tâm, tôi sẽ diệt sạch bọn chúng.
- [Snake]: Cảm ơn cậu. Tôi đặt hết niềm tin vào cậu đấy.

*(Hachi nuốt con dao vào người.)* (cảnh này sẽ làm 1 action để hiện cái img này lên, nhưng tôi chưa có vẽ)

- [Kati]: (giật mình) ...Cậu vừa làm cái gì vậy?!
- [Hachi]: Hehe, tuy không còn cơ thể như trước nhưng bù lại làm được trò này nè =))
- [Kati]: (mệt mỏi) ...Thôi tôi không hỏi nữa. Dù gì nãy giờ thì cũng điên quá rồi.
*(tele ra ngoài ngôi nhà lúc đầu, Text:"Cả hai cùng đi ra ngoài, chuẩn bị chiến đấu với lũ quỷ")*
*End cutscene.
*(Với sức mạnh của Hachi, cả hai càn quét sạch lũ quỷ quanh khu vực)*
- [Hachi]: Xong rồi, tôi nghĩ đấy là con cuối cùng rồi đấy. 
- [Kati]: Hộc, hộc... (thở dốc) ...Cũng may là còn sống. Việc di chuyển liên tục mệt quá đi. Tôi kiệt sức rồi.
- [Hachi]: Cũng phải ha, từ đầu tới giờ cậu chưa được nghỉ ngơi. Chúng ta về lại hầm trú ẩn để thông báo cho họ rồi nghỉ ngơi đi.
*(Tele về lại hầm trú ẩn, Text:"Cả hai cùng quay về hầm, đêm đó mọi người ăn mừng với nhau bỗng nhiên có một người lạ xuất hiện"  .)*

**Scene 1.4 — Gặp Usagi**

- [Hachi]: (ngạc nhiên) Ơ? Cậu là...
- [Usagi]: Hehe, lâu quá không gặp ha Hachi.
- [Kati]: (tò mò) Hai người quen nhau à? Mà này, cậu ở trong khu vực bị quỷ chiếm đóng làm gì vậy?
- [Usagi]: Tôi chỉ đơn giản là đi bán hàng thôi. Nếu người trong đây "hẹo" hết thì tôi mất khách, mà vật phẩm rơi ra từ lũ quỷ cũng có giá lắm đó.
- [Kati]: (nhìn Usagi với ánh mắt kỳ lạ) ...Tên này lạc quan kiểu gì vậy trời.
- [Usagi]: Tôi có thể cung cấp vũ khí, trang bị cho hai người — nhưng nhớ phải có thứ trao đổi nhé, tôi cũng cần tiền để sống mà.
- [Hachi]: Hahaha. Được thôi, tôi hiểu nỗi khổ của cậu mà.
- [Usagi]: Vậy bây giờ cứ đi tới làng Rohok trước đi, nơi đó chưa bị lũ quỷ xâm thực nặng như đây đâu. Nếu mục tiêu của hai người là thủ đô thì đó là đường tốt nhất rồi. Tôi sẽ gặp lại hai người sau — hy vọng lúc đó vẫn còn sống để mua đồ từ tôi ha.
- [Usagi]: Nhưng mà quên nữa, 2 người có muốn mua bán gì không ấy nhỉ?
* End cutscene.

- Cả 2 đi tới chỗ của Snake để chào tạm biệt anh ấy (Đây không phải action mà là đi tìm Triiger để mở cutscene của snake)
- [Snake]: Cậu quen Usagi à Hachi? Cậu ấy lần trước đã cung cấp nhu yếu phẩm cho chúng tôi, nếu không có cậu ấy chắc chúng tôi đã chết hết rồi.
- [Hachi]: Đúng vậy, cậu ấy nổi tiếng mà. Nhìn vậy thôi chứ cậu ta là một thiên tài trong các ma thuật ẩn thân đấy. Nên tới chắc đó giờ cậu ấy vẫn luôn giúp đỡ những người dân như mọi người ở đây.
- [Kati]: Tên quái gở này coi vậy mà cũng tốt đấy chứ.
- [Snake]: Nè, nghe nói hồi nãy các cậu định tới Rohok. Tôi tặng con dao cho các cậu đấy. Coi như quà tạm biệt, chúc các cậu may mắn.
- [Hachi]: Cảm ơn anh, tôi sẽ giải thoát quốc gia này.
*End cutscene.
---

## Chapter 2 — Rohok
Cutscene: Đầu chap
- [Kati]: (nhìn quanh) Đây là Rohok à. 
- [Kati]: Nè Hachi, chả phải Usagi nói là nơi này chưa bị quỷ chiếm đóng nặng sao? Sao mà hoàng tàn quá vậy?
- [Hachi]: Thật kì lạ, dấu vết ma lực còn rất mới có vẻ như lũ quỷ vừa mới tấn công thêm 1 đợt nữa.
- [Kati]: Để xem còn ai sống sót không, đi tìm dân làng thôi.
- [Hachi]: À mà nè, do tôi đã quen với việc kí sinh vào cơ thể cậu nên giờ tôi có thể sử dụng được một vài chiêu thức lại rồi.
- [Kati]: Hả? Vậy sao? Tôi tưởng cậu là chiến binh chứ không phải pháp sư.
- [Hachi]: Ừ thì đúng là việc đánh đấm tay chân đúng là sở trường của tôi, nhưng mà đâu ai cấm chiến binh không được học thêm phép thuật đâu.
- [Hachi]: Được thầy Shisa dạy dỗ, cũng như là học trong môi trường hoàng gia nên tôi cũng tiếp cận được nhiều kiến thức lắm.
- [Hachi]: Nhưng mà nè, tôi không phải cứ thích là dùng được đâu, tại đang ở trong cơ thể cậu mà. Nên 2 đứa phải đồng điệu với nhau. Tụi mình thử tập luyện 1 chút đi, chắc cũng không khó đâu.
- [Kati]: Để tôi thử xem.

- Cutscene: Trong thân cây lớn.
- [Hachi]: (ngó quanh) Nè tôi cảm nhận được ma lực của người sống. Có vẻ như họ đang ẩn nấp trong thân cây này.
- [Kati]: May quá vẫn còn người sống. Ta vào kiểm tra xem.
- [Kati]: Ơ, là trẻ con à.
- ShowObjectAction: Show obj npc
- [Npc]: (Kiệt sức) Ta sẽ chiến đấu với lũ các ngươi tới cùng lũ xấu xa.
- [Hachi]: Bình tĩnh đi, anh không phải quỷ. Anh tới đây để giải cứu vùng đất này. Tên anh là Hachi, còn đây là Kati. 
- [Hachi]: Tuy là nhìn tụi anh hơi kì cục nhưng mà chiến đấu thì tốt lắm đó.
- [Npc]: C-cảm ơn.
- [Kati]: Nè nè sao đấy nhóc?

- FadeUI: cậu nhóc ngất đi, có vẻ như cậu ấy đã cố quá rồi. Cả 2 cùng chăm sóc cho cậu ấy. Một lúc sau cậu ấy tỉnh lại.
- [Npc]: (mệt mỏi) Cảm ơn các người đã cứu em. Em tên là Minda, em là người duy nhất còn sống ở đây. Lũ quỷ đã quét sạch nơi này rồi.
- [Hachi]: Anh rất tiếc về chuyện đó. Anh biết em buồn nhưng em có biết bọn chúng đang tập trung ở đâu không?
- [Minda]: Từ đây đi về phía tây nam, hôm qua chúng đã tấn công từ phía đó và có thể căn cứ của chúng cùng ở đó thôi.
- [Kati]: Cảm ơn nhóc em dũng cảm lắm. Bọn anh sẽ quét sạch chúng cho em xem.
- [Hachi]: Ở đây nữa thì cũng nguy hiểm lắm, em biết đường tới ngôi làng Helga không?
- [Minda]: Dạ, em biết. 
- [Hachi]: Vậy thì nghe anh nói đây, nơi đó đã được bọn anh giải cứu rồi, vẫn còn vài người sống sót ở đó. Nếu em cứ ở đây thì nguy hiểm lắm, em cứ đi tới đó đi. Ở đây anh vẫn còn thức ăn cho em, hãy cố mà chạy tới đó đi.
- [Minda]: Dạ, em sẽ đi ngay. Cảm ơn các anh rất nhiều.

Cutscene *(Sau khi hạ được con quái mạnh nhất khu vực, nó làm rơi ra một tấm bản đồ.)*

- [Kati]: Ơ, cái này...
- ShowImgAction: Show img map
- [Hachi]: (cầm lên xem) Bản đồ mật độ tập trung tấn công của bọn quỷ! Có cái này thì tôi vạch được đường đi ngắn và ít rủi ro nhất tới thủ đô rồi.
- [Kati]: Đỡ phải đi lạc. Đi thôi.

*(Gặp lại Usagi.)*
- [Usagi]: Chào hai người, tôi rất vui vì thấy 2 cậu vẫn sống sót đó. 
- [Hachi]: Ở chào ông.
- [Usagi]: Xin lỗi vì thông tin tình báo trước đó của tôi đã lỗi thời rồi. Không ngờ bọn chúng lại tấn công nhanh vậy.
- [Hachi]: Chúng tôi vừa tìm được một bản đồ mật độ tập trung tấn công của bọn quỷ, nhờ đó mà tôi vạch được đường đi ngắn và ít rủi ro nhất tới thủ đô rồi.
- [Usagi]: Thật tuyệt vời, tôi có thể báo tin để các quốc gia đồng minh chuẩn bị tiếp viện và phản công. Phải sao chép thứ này lại mới được.
- [Kati]: Nè chúng tôi chật vật lắm mới sống được đó. Tại cậu hết, cậu phải chịu trách nhiệm cho chuyện này.
- [Usagi]: Hahaha, được thôi được thôi. Vừa đúng lúc tôi vừa nhập được nhiều đồ mới. Mời hai cậu xem thử.
---

## Chapter 3 — Trang trại Kuri
// Đây là Cutscene giới thiệu 
- [Hachi]: Kati này, phía trước là trang trại của ông Kuri. Ông ấy có truyền thống cung cấp lương thực cho cả quốc gia qua nhiều đời rồi.
- [Kati]: Tuyệt! Nếu giải quyết xong được quỷ ở chỗ này thì ta có thể loot được nhiều đồ ăn lắm đây hehehe.
- [Hachi]: Đúng vậy. Trang trại này chưa bị lũ quỷ phá quá nhiều, chắc tái chiếm lại được trước khi tiến sâu vào trung tâm.


// Đây là khi tới căn biệt thự của nhà Kuri.
- [Hachi]: (ngó quanh) Nè Kati, trong căn biệt thự này nồng nặc mùi quỷ. Có thể số lượng còn đông hơn tụi chúng ta đã xử nãy giờ
- [Kati]: Hả? Nơi nhỏ như này mà còn đông quỷ hơn sao?
- [Hachi]: Tôi không rõ nhưng trước đây tôi đã từng nghe qua về căn biệt thự và căn hầm của nơi đây. Có thể nó sâu và rộng hơn vẻ bề ngoài rất nhiều đó.
- [Kati]: Ngạc nhiên thật. Thôi ta cũng vào thôi.
Tele vào trong biệt thự không có text cho đoạn tele này.

*(Sau khi dọn sạch quái, họ tìm thấy Kuri và gia đình vẫn còn sống sót trong hầm trú ẩn.)* // Đây là 1 cutscene
Đi vào trigger:
- [Hachi]: Ơ căn phòng này.
Tele: Cả hai đi vào căn phòng, phát hiện gia đình Kuri và nhiều tùy tùng vẫn còn sống dù họ nhiều người đang bị thương.
- [Kuri]: Cậu là chiến binh của hoàng gia phải không?
- [Kuri]: Ta đã từng thấy mặt cậu trong cung điện trước đây.
- [Hachi]: Ông bình tĩnh dưỡng thương đi, không cần gắn sức quá đâu.
- [Hachi]: Tôi là Hachi, con nuôi của Shisa, còn người bạn nhỏ này là Kati. Do một số chuyện mà tôi thành ra như thế này.
- [Hachi]: Nhưng mà không sao đâu, tôi đã xử lí hết bọn quỷ ở ngoài trang trại và cả dưới hầm rồi. Từ giờ ông có thể yên tâm.
- [Kuri]: Cậu định tới thủ đô à? Có vài tình báo tôi phải nói cho cậu biết.
- [Kati]: Bất ngờ thật đấy! Tình trạng đất nước thế này mà ông vẫn nắm được tình trạng của thủ đô ư?
- [Kuri]: Cậu xem thường mạng lưới của gia đình tôi quá, nghĩ tôi chỉ là 1 địa chủ giàu có bình thường thôi à. Không phải tự nhiên nhà vua xem trọng tôi đến thế đâu nhé. 
- [Hachi]: Hehe, Thật là ngại quá.
- [Kuri]: Theo như những gì thuộc hạ của tôi đã báo cáo thì, tình trạng hiện tại của thủ đô rất tệ và quan trọng hơn là có một cổng dịch chuyển khổng lồ đang mở ra ở đó, nối thẳng đến vùng đất quỷ.
- [Hachi]: H-Hả, khoan đã... cổng dịch chuyển cỡ đó cần trình độ phép thuật cực cao với một mớ ma pháp khổng lồ — mà quan trọng nhất là phải có người thi triển ở CẢ HAI đầu cổng đó.
- [Kati]: Ý cậu là lũ quỷ không tự mở cổng một mình được hả?
- [Hachi]: Ừ đúng rồi bạn tôi. Tôi nghĩ là trình độ phép thuật của chúng chưa phát triển tới vậy để mở từ 1 đầu đâu. Vậy chỉ còn một khả năng thôi... có kẻ phản bội.
- [Kati]: Phản bội sao, nghe thật khó tin.
- [Hachi]: Ha, tôi cũng thấy vậy — quỷ với sinh vật thế giới này là hai nhánh khác hẳn nhau mà. Người thường học phép quỷ đã khó khăn vô cùng rồi, nói gì tới thi triển phép cỡ đó mà còn phải đồng điệu 2 đầu cổng.
- [Kuri]: Dù sao thì hai người cũng nên cẩn thận. Đường lui về đây vẫn còn, nếu lũ quỷ quay lại tấn công thì cứ về trú ẩn. Về lương thực thì cứ lấy thỏa thích đi nhé. Tôi không bận tâm đâu
- [Kati]: Cảm ơn ông. Chúng tôi đi tiếp đây.
AddItemAction(do chưa nghĩ ra nên add item gì nên để trống)
Tele ra lại bên ngoài: Cả hai rời khỏi trang trại, theo như tấm bản đồ thì đường tốt nhất tới thủ đô là đi qua thành phố Mira.

Cutscene: vừa ra ngoài đi 1 chút thì gặp lại Usagi
- [Usagi]: Chào hai người, trông 2 người có vẻ còn khỏe hơn cả lúc trước đó.
- [Hachi]: Haha, đúng vậy. Đồ ăn ở đây ngon quá và được mang theo rất nhiều nữa.
- [Usagi]: Đúng như tôi nghĩ, nơi này vẫn còn khá ổn so với những khu vực khác.
- [Kati] : Vào vấn đề chính đi, cậu có tin gì mới không?
- [Usagi]: Theo nhưng gì tôi thấy thì hiện tại thì bọn chúng tấn công cỏ vẻ chậm hơn trước nhưng tôi không nghĩ đây là một tin tốt.
- [Hachi]: Ý cậu là bình yên trước giông bão à?
- [Usagi]: Ừ, khả năng là tấm bản đồ đó của các cậu sẽ nhanh chóng lỗi thời thôi. Với tình hình hiện tại thì tôi cũng chả biết làm gì thêm.
- [Usagi]: Thôi thì giờ cứ theo như tấm bản đồ đó đi tiếp tới Mira đi. Nếu 2 cậu còn sống sau đó thì tôi sẽ cập nhật lại đường đi giúp cho.
- [Kati]: Cảm ơn cậu, giờ thì cậu còn gì mới để chúng tôi mua sắm không? Tôi có khá nhiều thứ giá trị đây.
- [Usagi]: Haha, tôi có vài món mới đây 2 cậu xem thử đi.
---

## Chapter 4 — Thành phố Mira

* Cutscene 1

- [Hachi]: (bất an) Nè Kati, chỗ này thật sự khác xa trí nhớ của tôi. Nơi đây từng nhộn nhịp lắm, sao giờ hoang tàn thế này... còn nồng nặc mana của lũ quỷ nữa.
- [Kati]: Cậu dắt tôi từ bất ngờ này tới bất ngờ nọ hoài nên giờ tôi cũng chai rồi, chẳng còn thấy bất ngờ được nữa. Thôi, việc của tụi mình hiện tại là gì?
- [Hachi]: Theo như những gì tôi đã được học thì những nơi có lượng mana quỷ dày đặc như thế này thì thường có lõi quỷ ở đâu đó. Đó là nguồn sống của quỷ nếu ở vùng đất không phải của chúng.
- [Hachi]: Nếu chúng ta phá được lõi quỷ thì lượng mana quỷ sẽ suy yếu, và bọn quỷ sẽ không còn sức mạnh để tấn công nữa.
- [Hachi]: Phải nhanh lên, nếu không lõi quỷ sẽ hòa làm một với nơi này, lúc đó phiền phức lắm.
- [Kati]: Ok đi thôi. Nhưng mà nó trông như thế nào vậy?
- [Hachi]: Lõi quỷ có nhiều hình dạng lắm. Thời sơ khai của chiến tranh hai thế giới, chúng chỉ là một khối thịt thôi, nhưng càng ngày càng tiến hóa để có hình dạng và trí óc riêng — nhiều con đại quỷ còn hòa làm một với chúng luôn.
- [Kati]: (nhún vai) Vậy là phiền thật rồi. Thôi kệ, đi thôi.
- [Hachi]: Đúng vậy lần này sẽ khó khăn hơn so với lúc trước nhiều đó.

* Cutscene 2
- [Kati]: Ôi cái đệch, b-buồn nôn quá. Cái quái gì vậy?
- [Hachi]: Đây là máu thịt của người và gia cầm trộn lẫn với nhau. Kinh khủng thật. Bọn chúng làm việc này để làm gì cơ chứ?
- [Kati]: Huệ... Tôi không muốn biết nữa.
- [Hachi]: Hình như là lũ quỷ đang muốn lăn khối thịt này đi đâu đó. Tôi nghe nói rằng bọn chúng thường có những tập tính cung cấp máu thịt cho lõi quỷ. 
- [Hachi]: Khả năng là cứ theo hướng này thì ta sẽ tìm được thôi.
- [Kati]: Nhanh đi thôi mùi ở đây tởm quá T_T

*Cutscene 3:
Tele: 2 bạn dần đi tới trung tâm thành phố, lượng mana quỷ ngày càng dày đặc hơn đến nổi cả Hachi cũng cảm thấy khó chịu.
- [Hachi]: (giọng run run) Nè Kati, hình như chúng ta tới rồi.
- [Kati]: Nè cậu có chắc là chúng ta thắng được không vậy.
- [Hachi]: Tôi không chắc nữa, đây là lần đầu tôi đối mặt với quỷ cấp cao thế này.
- [Kati]: Không sao đâu dù gì chúng ta cũng đã tới đây rồi, tiến lên thôi.


*(Sau khi hạ được lõi quỷ hình con dơi, họ gặp lại Usagi.)* (Nghĩa đánh xong rồi chạm phải trigger rồi tele ra ngoài xong mới gặp lại usagi trigger)

- [Usagi]: Hai người vẫn còn sống, tốt quá. Nhưng mà chắc nhìn tình trạng hiện tại chắc là cần nghĩ ngơi một chút rồi.
- [Hachi]: Có tin gì mới không?
- [Usagi]: Các quốc gia khác đang cố gắng giúp, nhưng khó tiến sâu vào đất nước này lắm. Nếu vậy thì hai người buộc phải tự tay đóng cổng dịch chuyển và phá lõi quỷ ở thủ đô để làm suy yếu phòng thủ của chúng.
- [Kati]: Vậy đi thẳng qua mấy thành phố lớn quanh thủ đô là được chứ gì?
- [Usagi]: Đừng, nguy hiểm lắm. Chúng đã chiếm được phần lớn các thành phố rồi.
- [Usagi]: Đi qua hang động Kynarite và khu rừng Mytharite đi — xa và hiểm trở hơn thật, nhưng an toàn hơn nhiều so với băng qua mấy thành phố kia.
- [Hachi]: Cảm ơn cậu, Usagi. Lần này chắc phải nhờ cậu tiếp tế thêm rồi.
- [Usagi]: Cứ để đó cho tôi, đi cẩn thận nhé.

---

## Chapter 5 — Hang động Kynarite

- [Kati]: (nhìn vào bóng tối hang động) ...Chỗ này âm u ghê.
- [Hachi]: (huýt sáo) Ôi dào, tối thui vậy chứ chắc cũng không có gì đâu — mà thôi lỡ có gì thì đánh thôi ha!
- [Kati]: (thở dài) Cậu nói câu nào cũng làm tôi thấy an tâm ghê á.

*(Sau khi phá hủy lõi quỷ trong hang, họ tiếp tục hành trình.)*

- [Hachi]: (búng tay) Xong một! Còn khu rừng Mytharite phía trước nữa thôi, cố lên!
- [Kati]: Nhanh lên đi, tôi không muốn ở lại chỗ này thêm giây nào.

---

## Chapter 6 — Khu rừng Mytharite

- [Kati]: Yên tĩnh quá vậy, không có gì trong này à?
- [Hachi]: Đừng chủ quan — chính vì yên tĩnh mới đáng sợ.

*(Sau khi phá hủy lõi quỷ trong rừng, trước mắt họ hiện ra thủ đô Hyvoria.)*

- [Kati]: (nhìn xa xăm) ...Đó là thủ đô à?
- [Hachi]: (giọng trầm xuống) Đúng vậy. Nhà của tôi.
- [Usagi]: (xuất hiện) Hai người tới rồi à. Quân đội từ các quốc gia đồng minh cũng có tiến triển tốt ngoài biên giới, đang theo con đường hai người mở ra rồi đó.
- [Kati]: Vậy là ổn chứ gì?
- [Usagi]: Chưa đâu, tốc độ vẫn không đủ. Nếu lõi quỷ thật sự hòa làm một với vùng đất này thì mọi chuyện sẽ khác hẳn. Hai người phải tăng tốc giải quyết cánh cổng và lõi quỷ trung tâm thôi.
- [Hachi]: (gật đầu) Hiểu rồi. Đi thôi Kati, lần này là trận cuối.

---

## Chapter 7 — Thủ đô Hyvoria

*(Đột nhập vào thủ đô, họ phát hiện phần lớn pháp sư ở đây đã bị quỷ điều khiển — thân xác họ thực chất đã chết, bị quỷ chiếm giữ, tôn thờ một con mắt khổng lồ.)*

- [Kati]: (rùng mình) ...Bọn này không còn là người nữa rồi.
- [Hachi]: (giọng nặng nề) Đúng vậy. Giải thoát cho họ thôi.

*(Sau khi tiêu diệt đám pháp sư bị hắc hóa, họ tìm thấy Mika đang bị thương nặng.)*

- [Hachi]: (hoảng hốt) Mika?! Là ông thật sao?!
- [Mika]: (yếu ớt) Hachi... cậu... còn sống sao...
- [Kati]: (vội chữa trị cho Mika) Ông cứ nằm yên đi đã.
- [Hachi]: Mika là cận thần của nhà vua, cũng là chiến binh lão luyện của vương quốc này. Ông ấy với cha tôi — thầy Shisa — được ví như hai cánh tay trái phải của quốc vương.
- [Mika]: (dần tỉnh) Hachi... tôi phải nói cho cậu biết... nguyên nhân của thảm họa này...
- [Hachi]: Nói đi, tôi nghe đây.
- [Mika]: Nguyên nhân... đến từ chính nhà vua.
- [Hachi]: (sững sờ) Cái gì?! Ông đang nói...
- [Mika]: Tôi biết khó tin lắm, chính tôi cũng chưa chấp nhận nổi. Nhưng tôi cảm nhận được sự hiện diện của ngài ở thủ đô này — mana của ngài đã biến thành thứ gì đó vô cùng kinh tởm.
- [Hachi]: (run giọng) ...Đúng là mana của nhà vua thật, nhưng nó bị ô nhiễm nặng nề. Làm sao một nhà vua có thể bị lũ quỷ thao túng để tự tay mở cổng dịch chuyển? Và tại sao lại làm vậy?
- [Kati]: Quỷ với người vốn đối đầu nhau như âm với dương mà, đúng không? Bắt tay với quỷ nghe vô lý quá.
- [Hachi]: Đúng, mà bị quỷ thao túng thì còn vô lý hơn — suốt thời gian trị vì, chưa ai từng phát hiện dấu hiệu gì bất thường ở ngài cả.

*(Họ lục soát phòng nhà vua, tìm ra manh mối: mẹ ngài mất ngay sau khi sinh ra ngài, gần như không ai biết gì về bà.)*

- [Kati]: (cầm bức chân dung cũ) Người phụ nữ này... là mẹ của vua à?
- [Hachi]: Chắc vậy. Đẹp thật, nhưng nghe nói bà rất ít khi xuất hiện trước công chúng.

*(Tiến vào phòng thí nghiệm, họ chạm trán lõi quỷ. Sau một trận chiến, mana của quỷ dần suy yếu — và họ phát hiện quốc vương đang bị giam bên trong.)*

- [Kanus]: (giọng khàn, tỉnh táo lại) ...Cuối cùng... tôi cũng lấy lại được ý chí của mình.
- [Hachi]: (chấn động) Là... nhà vua thật sao?!
- [Kanus]: Đúng vậy, Hachi. Con trai của Shisa... Xin lỗi vì đã để mọi chuyện đi đến nước này.
- [Kati]: Chuyện gì đã xảy ra với ngài vậy?
- [Kanus]: Mẹ ta... là người bị quỷ ám. Nhưng vì tình yêu, cha ta — quốc vương tiền nhiệm — vẫn chăm sóc cho bà tới cùng. Sau khi sinh ra ta, cha đã dùng mọi cách kiểm tra dấu hiệu của quỷ trên cơ thể ta, nhưng không lần nào phát hiện điều gì bất thường.
- [Kanus]: (cười khổ) Ai cũng tưởng vậy là xong rồi... Không ngờ chuyện này lại xảy ra. Chúng ta đã đánh giá quá thấp lũ quỷ này.
- [Hachi]: (nghẹn giọng) Vậy giờ phải làm sao để đóng cổng lại?
- [Kanus]: Ta sẽ chỉ cho hai người cách đóng cổng. Nhưng có một điều kiện duy nhất... ta phải chết.

*(Kanus hướng dẫn xong, tự kết liễu đời mình trước mặt cả hai.)*

- [Kati]: (lặng người) ...
- [Hachi]: (cúi đầu) ...Yên nghỉ đi, thưa bệ hạ.

*(Hai người đóng cổng theo chỉ dẫn của Kanus. Lũ quỷ trên khắp vùng đất suy yếu, quân tiếp viện tràn vào giải phóng thành phố.)*

- [Usagi]: (chạy tới) Xong rồi! Hai người làm được rồi!
- [Hachi]: (thở phào) ...Cuối cùng cũng xong.

*(Một thời gian sau, các pháp sư khôi phục lại cơ thể cũ của Hachi từ cái xác họ tìm được. Hachi trở thành vị vua mới của vương quốc.)*

- [Hachi]: (đứng trên cơ thể mới của mình, quay sang Kati) Kati này... cảm ơn cậu, vì tất cả.
- [Kati]: (nhún vai, cười nhẹ) Thôi khỏi cảm ơn, lo giữ lời hứa nghiên cứu phép dịch chuyển đưa tôi về thế giới thật đi là được.
- [Hachi]: (cười) Yên tâm, ta là vua rồi mà — chuyện đó dễ ợt.

---

## Ghi chú khi chuyển sang DialogueData

- Những đoạn có `*(...)*` là mô tả hành động/bối cảnh, KHÔNG đưa vào text
  của dòng thoại — dùng làm tham chiếu khi bạn chọn animation trigger /
  portrait biểu cảm cho từng CharacterProfile lúc build asset.
- Narrator chỉ còn đúng 1 dòng duy nhất trong toàn bộ 7 chapter (mở đầu
  Chapter 1) — đúng như yêu cầu giảm tối đa.
- Chapter 5–6 trong bản gốc gần như trống (tác giả tự nhận xét "hết rồi cu
  mong chờ gì cốt truyện ở mấy khu không có người sống") — tôi đã thêm vài
  câu banter ngắn giữa Kati/Hachi để không bị trống hoàn toàn, nhưng đây là
  phần MỎNG NHẤT, đáng để bạn viết thêm nếu muốn 2 chapter này có chiều sâu
  hơn (VD gặp thêm 1 NPC ẩn dật trong hang/rừng, hoặc lồng easter egg
  "Trứng phục sinh" vào đúng đây).
- "Trứng phục sinh" (Tezzy tìm nhà, Ifeelsoskibidi, Ne Ne, Trần Hải Bằng,
  Kiotakhai) CHƯA được đưa vào bản này — chưa rõ ý đồ gắn vào đâu, cần bạn
  nói rõ để viết tiếp.

## Ghi chú thiết kế (từ phản hồi ngày cập nhật gần nhất)

- **Giọng điệu Hachi**: chuyển sang vui tươi, bớt nghiêm nghị. Đã áp dụng
  mẫu cho Scene 1.2 (kể chuyện gốc), Chapter 3 (giải thích cổng dịch
  chuyển), Chapter 5-6 (banter). Các đoạn Chapter 7 (cao trào, Kanus tự
  sát) CỐ TÌNH giữ nghiêm túc vì đây là điểm cảm xúc chính của cả câu
  chuyện — kể cả nhân vật vui tính cũng nên trầm xuống ở đây, nếu bạn muốn
  nhẹ hơn nữa ở đoạn này thì nói rõ mức độ.
- **Bug đã sửa**: câu thoại "cảm nhận ma lực" trước đây gán nhầm cho Kati
  (Scene 1.3) — Kati là mèo thường, không có khả năng cảm mana. Đã chuyển
  toàn bộ phần cảm nhận mana sang Hachi trong suốt file, Kati chỉ còn phản
  ứng bằng giác quan thường (nhìn/nghe/ngửi thông thường).
- **Cấu trúc hội thoại Usagi = 2 phần tách biệt**: (1) một `DialogueData`
  hội thoại thông thường như NPC khác, (2) sau khi hội thoại kết thúc thì
  mở `Shop`/`Inventory` UI. Hệ thống `NPCDialogueTrigger` hiện tại CHƯA hỗ
  trợ nối tiếp 2 hành động này (chỉ gọi `DialogueManager.StartDialogue()`
  rồi dừng) — cần thêm callback `OnDialogueEnded` để tự mở Shop UI sau khi
  Usagi nói xong, hoặc tách thành 2 trigger riêng (nói chuyện xong thì hiện
  thêm 1 nút "Mở cửa hàng"). Đây là việc cần làm khi wire Usagi thật, CHƯA
  làm trong đợt sửa code Nhóm A trước đó.
- **Không cố định 1 chapter = 2 scene (village + level)**: đã verify trong
  code, xác nhận đúng là đang bị khóa cứng:
  - `ChapterDataSO` chỉ có 2 field: `mainSceneName` (1 scene level duy nhất)
    và `bossSceneName` (1 scene boss, optional) — không có list scene.
  - `ChapterManager.CompleteChapter()`/`CompleteBossChapter()` hardcode
    thẳng chuỗi `"Village"` để quay hub, KHÔNG đọc từ `ChapterDataSO` — mọi
    chapter bắt buộc dùng chung đúng 1 Village scene, không tùy biến được
    theo từng chapter.
  - Muốn hỗ trợ đa dạng scene hơn (VD nhiều sub-area trong 1 chapter, hoặc
    hub riêng theo chapter) cần đổi `ChapterDataSO` sang `List<string>` scene
    thay vì 2 field cố định, và bỏ hardcode `"Village"` trong
    `ChapterManager`. Đây là thay đổi kiến trúc — CHƯA sửa code, chỉ ghi
    nhận, cần bạn chốt thiết kế cụ thể (bao nhiêu scene/chapter, thứ tự đi
    qua ra sao) trước khi soạn prompt sửa.
