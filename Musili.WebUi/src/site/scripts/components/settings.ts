import { addEventListenerToList, nodeListToArray } from "../utils/dom-utils";

const SELECTED_CLASS = "selectable-btn--selected";

interface TracksSettings {
    tempo: string;
    genres: string[];
    lastId?: number;
}

export default class Settings {
    private root: HTMLElement;
    private tempos: HTMLElement[];
    private genres: HTMLElement[];

    public init(selector: string): void {
        this.root = document.querySelector(selector);
        const tempos = this.root.querySelectorAll(".js-tempo");
        const genres = this.root.querySelectorAll(".js-genre");
        addEventListenerToList(tempos, "click", e => { this.onTempoClick(e as MouseEvent); });
        addEventListenerToList(genres, "click", e => { this.onGenreClick(e as MouseEvent); });

        this.tempos = nodeListToArray(tempos);
        this.genres = nodeListToArray(genres);

        const storedSettings = this.getSettingsFromStore();
        this.tempos.forEach(btn => {
            if (storedSettings.tempo === btn.dataset.id) {
                btn.classList.add(SELECTED_CLASS);
                return;
            }
        });
        this.genres.forEach(btn => {
            if (storedSettings.genres.some(item => item === btn.dataset.id)) {
                btn.classList.add("selectable-btn--selected");
            }
        })
    }

    private getSettingsFromStore(): TracksSettings {
        return {
            tempo: "any",
            genres: ["classical", "electronic"],
        }
    }

    private onTempoClick(e: MouseEvent) {
        this.tempos.forEach(btn => {
            if (btn.dataset.id === (e.target as HTMLElement).dataset.id) {
                btn.classList.add(SELECTED_CLASS);
            } else {
                btn.classList.remove(SELECTED_CLASS);
            }
        });
    }

    private onGenreClick(e: MouseEvent) {
        const selectedGenres = this.genres.filter(item => item.classList.contains(SELECTED_CLASS));
        const target = e.target as HTMLElement;
        if (selectedGenres.length === 1 && selectedGenres[0].dataset.id === target.dataset.id) {
            return;
        }
        target.classList.toggle(SELECTED_CLASS);
    }
}