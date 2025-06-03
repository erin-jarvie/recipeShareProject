import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Recipe, RecipeService } from '../../services/recipe.service';

@Component({
  selector: 'app-recipe-detail',
  standalone: false,
  templateUrl: './recipe-detail.html',
  styleUrl: './recipe-detail.css'
})
export class RecipeDetailComponent implements OnInit {
  recipeId: number = 0;
  recipe: Recipe | null = null;

  constructor(private recipeService: RecipeService,
    private route: ActivatedRoute,
    private router: Router) { }

  ngOnInit() {
    this.recipeId = Number(this.route.snapshot.paramMap.get('id'));
    this.loadRecipe()
  }

  loadRecipe() {
    this.recipeService.getRecipe(this.recipeId).subscribe(data => {
      this.recipe = data
    });
  }
}
