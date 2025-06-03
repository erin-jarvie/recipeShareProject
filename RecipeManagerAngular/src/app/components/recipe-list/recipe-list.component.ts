import { Component, OnInit } from '@angular/core';
import { DietaryTag, GetRecipesCommand, Recipe, RecipeService } from '../../services/recipe.service';
import { Router } from '@angular/router';

declare const bootstrap: any;

@Component({
  selector: 'app-recipe-list',
  standalone: false,
  templateUrl: './recipe-list.html',
  styleUrl: './recipe-list.css'
})
export class RecipeListComponent implements OnInit {
  recipes: Recipe[] = [];

  tags: DietaryTag[] = [];
  selectedTagId: number | null = null;
  searchString: string = ''

  actionItem: Recipe | null = null;
  constructor(private recipeService: RecipeService,
  private router: Router) { }

  ngOnInit(): void {
    this.loadRecipes()
  }

  loadRecipes() {
    const command: GetRecipesCommand = {
      tagId: this.selectedTagId,
      searchString: this.searchString
    };
    this.recipeService.getRecipes(command).subscribe(data => {
      this.recipes = data.recipes;
      this.tags = data.dietaryTags;
    });
  }

  editRecipe(recipe: Recipe) {
    this.router.navigate(['/edit', recipe.id]);
  }

  viewRecipe(recipe : Recipe) {
    this.router.navigate(['/recipe', recipe.id]);
  }

  deleteRecipe() {
    if (!this.actionItem) return;

    this.recipeService.deleteRecipe(this.actionItem.id).subscribe(() => {
      this.loadRecipes();

      const modalEl = document.getElementById('deleteConfirmModal');
      if (modalEl) {
        bootstrap.Modal.getInstance(modalEl)?.hide();
      }
    });
  }

  addNewRecipe() {
    this.router.navigate(['/add']);
  }

  showFiltersPopup() {
    const modalEl = document.getElementById('filtersModal');
    if (modalEl) {
      new bootstrap.Modal(modalEl).show();
    }
  }

  applyFilters() {
    this.loadRecipes()
  }

  clearFilters() {
    this.selectedTagId = null;
    this.searchString = '';
    this.loadRecipes();
  }

  showConfirmDeletePopup(recipe: Recipe) {
    this.actionItem = recipe;

    const modalEl = document.getElementById('deleteConfirmModal');
    if (modalEl) {
      const modal = new bootstrap.Modal(modalEl);
      modal.show();
    }
  }
}
