import { MenuRecommedRes } from "../menu";

export type Bubble = {
    role: 'user' | 'agent';
    text: string | undefined;
    meta?: Partial<MenuRecommedRes>
}

// export const DUMMY_MENU_RECOMMEND_RES: MenuRecommedRes = {
//     query: 'حالم بده یه نوشیدنی شیرین میخوام',
//     topN: 5,
//     messageFa: 'می‌فهممت 😌 برای یه حس بهتر، این چندتا گزینه‌ی شیرین و خوشمزه رو پیشنهاد میدم:',
//     candidates: [
//         {
//             id: 'cnd_001',
//             catgeryNameFa: 'نوشیدنی گرم',
//             persianName: 'هات چاکلت کلاسیک',
//             englishName: 'Classic Hot Chocolate',
//             ingredients: 'شکلات، شیر، خامه، پودر کاکائو',
//             imageUrl: 'https://picsum.photos/seed/hotchocolate/640/420',
//             priceValue: 125000,
//             score: 0.92
//         },
//         {
//             id: 'cnd_002',
//             catgeryNameFa: 'نوشیدنی سرد',
//             persianName: 'آیس موکا',
//             englishName: 'Iced Mocha',
//             ingredients: 'قهوه، شیر، شکلات، یخ',
//             imageUrl: 'https://picsum.photos/seed/icedmocha/640/420',
//             priceValue: 145000,
//             score: 0.89
//         },
//         {
//             id: 'cnd_003',
//             catgeryNameFa: 'نوشیدنی سرد',
//             persianName: 'میلک‌شیک وانیل',
//             englishName: 'Vanilla Milkshake',
//             ingredients: 'بستنی وانیلی، شیر، وانیل',
//             imageUrl: 'https://picsum.photos/seed/vanillashake/640/420',
//             priceValue: 160000,
//             score: 0.87
//         },
//         {
//             id: 'cnd_004',
//             catgeryNameFa: 'نوشیدنی گرم',
//             persianName: 'لاته کارامل',
//             englishName: 'Caramel Latte',
//             ingredients: 'قهوه، شیر، سیروپ کارامل',
//             imageUrl: 'https://picsum.photos/seed/caramellatte/640/420',
//             priceValue: 135000,
//             score: 0.84
//         },
//         {
//             id: 'cnd_005',
//             catgeryNameFa: 'دسر',
//             persianName: 'چیزکیک',
//             englishName: 'Cheesecake',
//             ingredients: 'پنیر خامه‌ای، بیسکوییت، شکر',
//             imageUrl: 'https://picsum.photos/seed/cheesecake/640/420',
//             priceValue: 175000,
//             score: 0.81
//         }
//     ]
// };


// export const DUMMY_CHAT_HISTORY: Bubble[] = [
//   { role: 'agent', text: 'سلام! چی دوست داری امروز سفارش بدی؟ 😊' },
//   { role: 'user', text: 'حالم بده و یه نوشیدنی شیرین میخوام' },
//   {
//     role: 'agent',
//     text: DUMMY_MENU_RECOMMEND_RES.messageFa,
//     meta: DUMMY_MENU_RECOMMEND_RES
//   }
// ];
