import { Component } from '@angular/core';
import { CognitoService } from '../cognito.service';
import { HotelService } from '../hotel.service';
import { IHotel } from '../interfaces/IHotel';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.scss'],
})
export class AdminComponent {
  selectedFile: File | null = null;
  hotel: IHotel;

  constructor(
    private hotelService: HotelService,
    private cognitoService: CognitoService
  ) {
    this.hotel = {} as IHotel;
  }

  onFileChange(event: any) {
    this.selectedFile = event.target.files[0];
  }

  uploadFile() {
    this.cognitoService.getSession().then((session: any) => {
      this.cognitoService.getUser().then((user: any) => {
        this.hotel.userId = user.attributes.sub;
        this.hotel.idToken = session.accessToken.jwtToken;

        if (this.selectedFile) {
          this.hotelService.saveHotel(this.hotel, this.selectedFile).subscribe(
            (response) => {
              console.log('File uploaded successfully!', response);
            },
            (error) => {
              console.error('Error uploading file:', error);
            }
          );
        }
      });
    });
  }
}
