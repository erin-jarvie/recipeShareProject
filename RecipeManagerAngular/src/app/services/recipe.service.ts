import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

const API_URL = 'https://localhost:7063/api/recipes'; 

export interface RecipesViewModel {
  recipes: Recipe[];
  dietaryTags: DietaryTag[];
}

export interface RecipeFormViewModel {
  recipe: Recipe;
  dietaryTags: DietaryTag[];
}
export interface Ingredient {
  id: number;
  name: string;
  amount: number;
  unit: string;
}

export interface DietaryTag {
  id: number;
  name: string;
}

export interface Recipe {
  id: number;
  name: string;
  description: string;
  method: string;
  cookingTimeMins: number;
  noOfServings: number;
  dietaryTagId?: number;
  ingredients: Ingredient[];
  dietaryTag?: DietaryTag;
}

export interface GetRecipesCommand {
  tagId: number | null;
  searchString?: string;
}


@Injectable({
  providedIn: 'root'
})
export class RecipeService {

  constructor(private http: HttpClient) { }

  getRecipes(command: GetRecipesCommand): Observable<RecipesViewModel> {
    let params = new HttpParams();

    if (command.tagId) {
      params = params.set('TagId', command.tagId);
    }

    if (command.searchString) {
      params = params.set('SearchString', command.searchString);
    }

    return this.http.get<RecipesViewModel>(API_URL, { params });
  }
  

  getRecipeForm(id?: number): Observable<RecipeFormViewModel> {
    let params = new HttpParams();

    if (id) {
      params = params.set('Id', id);
    }
    return this.http.get<RecipeFormViewModel>(`${API_URL}/form`, { params });
  }

  getRecipe(id: number): Observable<Recipe> {
    return this.http.get<Recipe>(`${API_URL}/${id}`);
  }

  addRecipe(recipe: Recipe): Observable<Recipe> {
    return this.http.post<Recipe>(API_URL, recipe);
  }

  updateRecipe(id: number, recipe: Recipe): Observable<Recipe> {
    return this.http.put<Recipe>(`${API_URL}/${id}`, recipe);
  }

  deleteRecipe(id: number): Observable<void> {
    return this.http.delete<void>(`${API_URL}/${id}`);
  }
}

