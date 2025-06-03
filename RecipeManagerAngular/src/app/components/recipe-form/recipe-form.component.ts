import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DietaryTag, Recipe, RecipeService } from '../../services/recipe.service';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-recipe-form',
  standalone: false,
  templateUrl: './recipe-form.html',
  styleUrl: './recipe-form.css'
})
export class RecipeFormComponent implements OnInit {

  recipeId?: number;
  recipe: Recipe | null = null;
  tags: DietaryTag[] = [];
  form!: FormGroup;
  constructor(private recipeService: RecipeService,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private toastr: ToastrService,
    private router: Router) { }

  ngOnInit() {
    this.form = this.fb.group({
      name: ['', Validators.required],
      description: [''],
      method: [''],
      cookingTimeMins: [0, [Validators.required, Validators.min(1)]],
      noOfServings: [1, [Validators.required, Validators.min(1)]],
      dietaryTagId: [null],
      ingredients: this.fb.array([])
    });

    this.recipeId = this.route.snapshot.paramMap.get('id') ? Number(this.route.snapshot.paramMap.get('id')) : undefined;

    this.loadRecipe();
    
  }

  loadRecipe() {
    this.recipeService.getRecipeForm(this.recipeId).subscribe(data => {
      this.tags = data.dietaryTags;
      if (this.recipeId) {
        this.form.patchValue(data.recipe);
        data.recipe.ingredients.forEach(i => this.addIngredient(i));
      } else {
        this.addIngredient();
      }
    });
  }

  get ingredients(): FormArray {
    return this.form.get('ingredients') as FormArray;
  }

  addIngredient(ingredient?: any) {
    this.ingredients.push(this.fb.group({
      name: [ingredient?.name || '', Validators.required],
      amount: [ingredient?.amount || 0, [Validators.min(0.01)]],
      unit: [ingredient?.unit || '']
    }));
  }

  removeIngredient(index: number) {
    this.ingredients.removeAt(index);
  }

  submit() {
    if (this.form.invalid) {
      this.toastr.error('Please populate all required fields')
    };

    const recipeData = this.form.value;

    if (this.recipeId) {
      this.recipeService.updateRecipe(this.recipeId, recipeData).subscribe(() => {
        this.toastr.success("Recipe updated")
      });
    } else {
      this.recipeService.addRecipe(recipeData).subscribe(() => {
        this.toastr.success("Recipe Added")
        this.router.navigate(['/']);
      });
    }
  }

  back() {
    this.router.navigate(['/']);
  }

}
