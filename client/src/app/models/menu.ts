export class MenuRecommnedReq
{
    query: string = '';
    topN: number = 10;
    topK: number = 3;
}

export interface MenuRecommedRes
{
    query: string;
    topN: number;
    messageFa: string;
    candidates: MenuCandiate[];
}

export interface MenuCandiate
{
    id: string;
    catgeryNameFa: string;
    persianName: string;
    englishName: string;
    ingredients: string;
    imageUrl: string;
    priceValue: number;
    sizes: MenuPriceSize[];
    score: number;
}

export interface MenuPriceSize
{
    size: string;
    price: string;
    priceValue: number;
}